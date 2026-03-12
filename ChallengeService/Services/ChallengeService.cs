using ChallengeService.Dtos;
using ChallengeService.Dtos.External;
using ChallengeService.Enums;
using ChallengeService.Models;
using ChallengeService.Repository;
using ChallengeService.RestClient;

namespace ChallengeService.Services
{
    public class ChallengeService : IChallengeService
    {
        private readonly IRepository<Challenge> _challengeRepository;
        private readonly IRepository<UserChallenge> _userChallengeRepository;
        private readonly IRepository<ChallengePayment> _challengePaymentRepository;
        private readonly ServiceClient _serviceClient;

        public ChallengeService(
            IRepository<Challenge> challengeRepository,
            IRepository<UserChallenge> userChallengeRepository,
            IRepository<ChallengePayment> challengePaymentRepository,
            ServiceClient serviceClient)
        {
            _challengeRepository = challengeRepository;
            _userChallengeRepository = userChallengeRepository;
            _challengePaymentRepository = challengePaymentRepository;
            _serviceClient = serviceClient;
        }

        // ----------------------------------------------------------------
        // Créer un défi — valide que le créateur a des dettes dans DebtService
        // ----------------------------------------------------------------
        public async Task<ChallengeDto> CreateAsync(CreateChallengeDto dto)
        {
            await ValidateUserHasDebtsAsync(dto.CreatorUserId);

            var challenge = new Challenge
            {
                Title = dto.Title,
                Description = dto.Description,
                TargetAmount = dto.TargetAmount,
                DueDate = dto.DueDate,
                CreatorUserId = dto.CreatorUserId,
                Status = ChallengeStatus.ACTIVE,
                CreatedAt = DateTime.UtcNow
            };

            await _challengeRepository.AddAsync(challenge);
            await _challengeRepository.SaveChangesAsync();

            // Le créateur rejoint automatiquement son propre défi
            var participation = new UserChallenge
            {
                ChallengeId = challenge.Id,
                UserId = dto.CreatorUserId,
                AmountPaid = 0,
                JoinedAt = DateTime.UtcNow
            };

            await _userChallengeRepository.AddAsync(participation);
            await _userChallengeRepository.SaveChangesAsync();

            return await BuildChallengeDtoAsync(challenge);
        }

        // ----------------------------------------------------------------
        // Récupérer tous les défis
        // ----------------------------------------------------------------
        public async Task<List<ChallengeDto>> GetAllAsync()
        {
            var challenges = await _challengeRepository.GetAllAsync();
            var result = new List<ChallengeDto>();

            foreach (var c in challenges)
                result.Add(await BuildChallengeDtoAsync(c));

            return result;
        }

        // ----------------------------------------------------------------
        // Récupérer un défi par ID
        // ----------------------------------------------------------------
        public async Task<ChallengeDto?> GetByIdAsync(int id)
        {
            var challenge = await _challengeRepository.GetByIdAsync(id);
            if (challenge == null) return null;
            return await BuildChallengeDtoAsync(challenge);
        }

        // ----------------------------------------------------------------
        // Rejoindre un défi — valide que l'user a des dettes dans DebtService
        // ----------------------------------------------------------------
        public async Task<UserChallengeDto> JoinAsync(int challengeId, JoinChallengeDto dto)
        {
            var challenge = await _challengeRepository.GetByIdAsync(challengeId)
                ?? throw new KeyNotFoundException($"Challenge {challengeId} not found.");

            if (challenge.Status == ChallengeStatus.COMPLETED)
                throw new InvalidOperationException("Cannot join a completed challenge.");

            // Valider que l'user a au moins une dette dans DebtService
            await ValidateUserHasDebtsAsync(dto.UserId);

            // Vérifier que l'utilisateur n'a pas déjà rejoint
            var existing = await _userChallengeRepository.FindAsync(
                uc => uc.ChallengeId == challengeId && uc.UserId == dto.UserId);

            if (existing.Any())
                throw new InvalidOperationException($"User {dto.UserId} has already joined this challenge.");

            var participation = new UserChallenge
            {
                ChallengeId = challengeId,
                UserId = dto.UserId,
                AmountPaid = 0,
                JoinedAt = DateTime.UtcNow
            };

            await _userChallengeRepository.AddAsync(participation);
            await _userChallengeRepository.SaveChangesAsync();

            return MapToUserChallengeDto(participation);
        }

        // ----------------------------------------------------------------
        // Enregistrer un paiement depuis PaymentService
        //
        // Chaîne de validation :
        //   1. Vérifier que l'user a rejoint le défi
        //   2. Récupérer le paiement depuis PaymentService via PaymentId
        //   3. Récupérer la dette depuis DebtService via payment.DebtId
        //   4. Vérifier que debt.UserId == dto.UserId (ownership)
        //   5. Anti double-comptage : vérifier que ce PaymentId n'est pas déjà utilisé
        //   6. Enregistrer le ChallengePayment et mettre à jour AmountPaid
        // ----------------------------------------------------------------
        public async Task<UserChallengeDto> RecordPaymentAsync(int challengeId, RecordChallengePaymentDto dto)
        {
            var challenge = await _challengeRepository.GetByIdAsync(challengeId)
                ?? throw new KeyNotFoundException($"Challenge {challengeId} not found.");

            if (challenge.Status == ChallengeStatus.COMPLETED)
                throw new InvalidOperationException("Challenge is already completed.");

            // 1. Vérifier que l'user a bien rejoint ce défi
            var participations = await _userChallengeRepository.FindAsync(
                uc => uc.ChallengeId == challengeId && uc.UserId == dto.UserId);

            var participation = participations.FirstOrDefault()
                ?? throw new InvalidOperationException($"User {dto.UserId} has not joined challenge {challengeId}.");

            // 2. Récupérer le paiement depuis PaymentService
            var payment = await _serviceClient.GetAsync<ExternalPaymentDto>(
                "PaymentService", $"/api/v1/payments/{dto.PaymentId}")
                ?? throw new KeyNotFoundException($"Payment {dto.PaymentId} not found in PaymentService.");

            // 3. Récupérer la dette depuis DebtService pour vérifier la propriété
            var debt = await _serviceClient.GetAsync<ExternalDebtDto>(
                "DebtService", $"/api/Debt/{payment.DebtId}")
                ?? throw new KeyNotFoundException($"Debt {payment.DebtId} not found in DebtService.");

            // 4. Valider que la dette appartient bien à cet utilisateur
            if (debt.UserId != dto.UserId)
                throw new UnauthorizedAccessException(
                    $"Payment {dto.PaymentId} does not belong to user {dto.UserId}.");

            // 5. Anti double-comptage : ce PaymentId ne doit pas déjà exister dans ce défi
            var alreadyUsed = await _challengePaymentRepository.FindAsync(
                cp => cp.ChallengeId == challengeId && cp.PaymentId == dto.PaymentId);

            if (alreadyUsed.Any())
                throw new InvalidOperationException(
                    $"Payment {dto.PaymentId} has already been recorded in challenge {challengeId}.");

            // 6. Enregistrer le ChallengePayment (traçabilité + anti double-comptage)
            var challengePayment = new ChallengePayment
            {
                ChallengeId = challengeId,
                UserId = dto.UserId,
                PaymentId = dto.PaymentId,
                Amount = payment.Amount,
                RecordedAt = DateTime.UtcNow
            };

            await _challengePaymentRepository.AddAsync(challengePayment);
            await _challengePaymentRepository.SaveChangesAsync();

            // 7. Mettre à jour le total de l'utilisateur dans ce défi
            participation.AmountPaid += payment.Amount;
            await _userChallengeRepository.UpdateAsync(participation);
            await _userChallengeRepository.SaveChangesAsync();

            // 8. Vérifier si l'objectif collectif est atteint
            var allParticipations = await _userChallengeRepository.FindAsync(
                uc => uc.ChallengeId == challengeId);

            double totalPaid = allParticipations.Sum(uc => uc.AmountPaid);

            if (totalPaid >= challenge.TargetAmount)
            {
                challenge.Status = ChallengeStatus.COMPLETED;
                await _challengeRepository.UpdateAsync(challenge);
                await _challengeRepository.SaveChangesAsync();
            }

            return MapToUserChallengeDto(participation);
        }

        // ----------------------------------------------------------------
        // Progression globale du défi
        // ----------------------------------------------------------------
        public async Task<ChallengeProgressDto?> GetProgressAsync(int challengeId)
        {
            var challenge = await _challengeRepository.GetByIdAsync(challengeId);
            if (challenge == null) return null;

            var participants = await _userChallengeRepository.FindAsync(
                uc => uc.ChallengeId == challengeId);

            double totalPaid = participants.Sum(uc => uc.AmountPaid);
            double progress = challenge.TargetAmount > 0
                ? Math.Round(totalPaid / challenge.TargetAmount * 100, 2)
                : 0;

            return new ChallengeProgressDto
            {
                ChallengeId = challenge.Id,
                Title = challenge.Title,
                TargetAmount = challenge.TargetAmount,
                TotalPaid = Math.Round(totalPaid, 2),
                ProgressPercent = Math.Min(progress, 100),
                ParticipantCount = participants.Count,
                Status = challenge.Status,
                DueDate = challenge.DueDate,
                Leaderboard = BuildLeaderboard(participants)
            };
        }

        // ----------------------------------------------------------------
        // Leaderboard du défi
        // ----------------------------------------------------------------
        public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int challengeId)
        {
            var participants = await _userChallengeRepository.FindAsync(
                uc => uc.ChallengeId == challengeId);

            return BuildLeaderboard(participants);
        }

        // ----------------------------------------------------------------
        // Helpers privés
        // ----------------------------------------------------------------

        /// <summary>
        /// Vérifie que l'utilisateur a au moins une dette dans DebtService.
        /// Garantit que seuls les utilisateurs ayant des dettes enregistrées
        /// peuvent créer ou rejoindre un défi.
        /// </summary>
        private async Task ValidateUserHasDebtsAsync(int userId)
        {
            var allDebts = await _serviceClient.GetListAsync<ExternalDebtDto>("DebtService", "/api/Debt");
            var userDebts = allDebts.Where(d => d.UserId == userId).ToList();

            if (!userDebts.Any())
                throw new InvalidOperationException(
                    $"User {userId} has no debts registered in DebtService. Register at least one debt first.");
        }

        private async Task<ChallengeDto> BuildChallengeDtoAsync(Challenge challenge)
        {
            var participants = await _userChallengeRepository.FindAsync(
                uc => uc.ChallengeId == challenge.Id);

            double totalPaid = participants.Sum(uc => uc.AmountPaid);
            double progress = challenge.TargetAmount > 0
                ? Math.Round(totalPaid / challenge.TargetAmount * 100, 2)
                : 0;

            return new ChallengeDto
            {
                Id = challenge.Id,
                Title = challenge.Title,
                Description = challenge.Description,
                TargetAmount = challenge.TargetAmount,
                TotalPaid = Math.Round(totalPaid, 2),
                ProgressPercent = Math.Min(progress, 100),
                DueDate = challenge.DueDate,
                CreatorUserId = challenge.CreatorUserId,
                ParticipantCount = participants.Count,
                Status = challenge.Status,
                CreatedAt = challenge.CreatedAt
            };
        }

        private static List<LeaderboardEntryDto> BuildLeaderboard(List<UserChallenge> participants)
        {
            return participants
                .OrderByDescending(uc => uc.AmountPaid)
                .Select((uc, index) => new LeaderboardEntryDto
                {
                    Rank = index + 1,
                    UserId = uc.UserId,
                    AmountPaid = Math.Round(uc.AmountPaid, 2),
                    JoinedAt = uc.JoinedAt
                })
                .ToList();
        }

        private static UserChallengeDto MapToUserChallengeDto(UserChallenge uc) => new()
        {
            Id = uc.Id,
            ChallengeId = uc.ChallengeId,
            UserId = uc.UserId,
            AmountPaid = Math.Round(uc.AmountPaid, 2),
            JoinedAt = uc.JoinedAt
        };
    }
}
