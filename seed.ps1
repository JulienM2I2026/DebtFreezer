# =============================================================
#  DebtFreezer - Script de seed de données de test
#  Usage : .\seed.ps1
#  Pré-requis : Gateway accessible sur http://localhost:5099
# =============================================================

$BASE = "http://localhost:5099"
$headers = @{ "Content-Type" = "application/json" }

function Invoke-Api($method, $url, $body = $null, $token = $null) {
    $h = @{ "Content-Type" = "application/json" }
    if ($token) { $h["Authorization"] = "Bearer $token" }
    $params = @{ Method = $method; Uri = "$BASE$url"; Headers = $h; ErrorAction = "Stop" }
    if ($body) { $params["Body"] = ($body | ConvertTo-Json -Depth 5) }
    try {
        $resp = Invoke-RestMethod @params
        return $resp
    } catch {
        Write-Host "  [ERREUR] $method $url : $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

function Register-And-Login($email, $fullName, $password, $budget) {
    Write-Host "`n>>> Inscription : $fullName ($email)" -ForegroundColor Cyan

    $user = Invoke-Api "POST" "/api/v1/Auth/register" @{
        email = $email
        fullName = $fullName
        password = $password
        monthlyRepaymentBudget = $budget
        repaymentStrategy = 0
    }

    if (-not $user) {
        Write-Host "  Utilisateur existant ? Tentative de connexion..." -ForegroundColor Yellow
    }

    $auth = Invoke-Api "POST" "/api/v1/Auth/login" @{
        email = $email
        password = $password
    }

    if ($auth -and $auth.accessToken) {
        Write-Host "  [OK] Connecté. Token obtenu." -ForegroundColor Green
        return $auth.accessToken
    } else {
        Write-Host "  [ECHEC] Impossible de se connecter." -ForegroundColor Red
        return $null
    }
}

function Create-Debt($token, $creditor, $originalAmount, $interestRate, $dueDateStr, $type) {
    $debt = Invoke-Api "POST" "/api/v1/Debt" @{
        creditor = $creditor
        originalAmount = $originalAmount
        remainingAmount = $originalAmount
        interestRate = $interestRate
        dueDate = $dueDateStr
        type = $type
        status = 0
    } -token $token

    if ($debt -and $debt.id) {
        Write-Host "  [OK] Dette créée : '$creditor' ($originalAmount €) → ID $($debt.id)" -ForegroundColor Green
        return $debt.id
    } else {
        Write-Host "  [ECHEC] Création dette '$creditor'" -ForegroundColor Red
        return $null
    }
}

function Create-Payment($token, $debtId, $amount, $notes) {
    $payment = Invoke-Api "POST" "/api/v1/Payment" @{
        debtId = $debtId
        amount = $amount
        paymentDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        notes = $notes
    } -token $token

    if ($payment) {
        Write-Host "  [OK] Paiement de $amount € sur dette #$debtId" -ForegroundColor Green
    } else {
        Write-Host "  [ECHEC] Paiement sur dette #$debtId" -ForegroundColor Red
    }
}

# ==============================================================
#  UTILISATEUR 1 — Alice Martin
# ==============================================================
$token1 = Register-And-Login "alice@debtfreezer.com" "Alice Martin" "Password123!" 800

if ($token1) {
    Write-Host "`n  Création des dettes pour Alice..." -ForegroundColor Cyan

    $d1 = Create-Debt $token1 "BNP Paribas"      12000  4.5  "2028-06-01" 3  # MORTGAGE (3)
    $d2 = Create-Debt $token1 "Cetelem"            3500  6.9  "2026-12-01" 1  # PERSONAL_LOAN (1)
    $d3 = Create-Debt $token1 "Caisse d'Epargne"   8000  3.2  "2027-09-15" 4  # AUTO_LOAN (4)

    Write-Host "`n  Création des paiements pour Alice..." -ForegroundColor Cyan

    if ($d1) {
        Create-Payment $token1 $d1 400 "Mensualité janvier"
        Create-Payment $token1 $d1 400 "Mensualité février"
    }
    if ($d2) {
        Create-Payment $token1 $d2 150 "Remboursement partiel"
    }
    if ($d3) {
        Create-Payment $token1 $d3 250 "Mensualité mars"
        Create-Payment $token1 $d3 250 "Mensualité avril"
    }
}

# ==============================================================
#  UTILISATEUR 2 — Bob Dupont
# ==============================================================
$token2 = Register-And-Login "bob@debtfreezer.com" "Bob Dupont" "Password123!" 500

if ($token2) {
    Write-Host "`n  Création des dettes pour Bob..." -ForegroundColor Cyan

    $d4 = Create-Debt $token2 "Cofidis"          2000  18.0 "2026-08-01" 0  # CREDIT_CARD (0)
    $d5 = Create-Debt $token2 "Société Générale" 15000  2.8 "2030-01-01" 3  # MORTGAGE (3)
    $d6 = Create-Debt $token2 "Younited Credit"   1200  9.5 "2025-11-01" 2  # STUDENT_LOAN (2)

    Write-Host "`n  Création des paiements pour Bob..." -ForegroundColor Cyan

    if ($d4) {
        Create-Payment $token2 $d4 100 "Premier versement"
        Create-Payment $token2 $d4 200 "Solde partiel"
    }
    if ($d5) {
        Create-Payment $token2 $d5 600 "Mensualité"
    }
    if ($d6) {
        Create-Payment $token2 $d6 80  "Remboursement étudiant"
        Create-Payment $token2 $d6 120 "Remboursement étudiant"
    }
}

# ==============================================================
#  UTILISATEUR 3 — Clara Petit (utilisateur de test minimal)
# ==============================================================
$token3 = Register-And-Login "clara@debtfreezer.com" "Clara Petit" "Password123!" 300

if ($token3) {
    Write-Host "`n  Création des dettes pour Clara..." -ForegroundColor Cyan

    $d7 = Create-Debt $token3 "Floa Bank" 500 12.0 "2025-07-01" 5   # OTHER (5)

    if ($d7) {
        Create-Payment $token3 $d7 50 "Paiement test"
    }
}

Write-Host "`n============================================" -ForegroundColor White
Write-Host "  Seed terminé !" -ForegroundColor Green
Write-Host "  Comptes créés :" -ForegroundColor White
Write-Host "    alice@debtfreezer.com  / Password123!" -ForegroundColor White
Write-Host "    bob@debtfreezer.com    / Password123!" -ForegroundColor White
Write-Host "    clara@debtfreezer.com  / Password123!" -ForegroundColor White
Write-Host "============================================`n" -ForegroundColor White
