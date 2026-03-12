# PROJET FIL ROUGE - POEI .NET
## "DEBTFREEZER" - Plateforme de Gestion des Dettes Collaboratif

---

## VISION DU PROJET

**Objectif:** Développer une plateforme SaaS permettant aux utilisateurs de tracker leurs dettes, créer des plans de remboursement collaboratifs, et se motiver en groupe pour devenir debt-free.

**Cible:** Particuliers endettés, groupes d'amis/famille, conseillers financiers.

**Approche:** MVP sans IA puis évolution avec modèles LLM externes (Claude, GPT).

---

## CONTEXTE METIER

### Problématique
Les utilisateurs ont souvent plusieurs dettes (cartes crédit, prêts personnels, prêts étudiants) avec:
- Montants différents
- Taux d'intérêt variables
- Dates d'échéance différentes
- Difficulté à organiser un plan global

**Solution:** Une plateforme qui:
1. Centralise toutes les dettes au même endroit
2. Propose des stratégies de remboursement optimisées
3. Suit la progression avec notifications
4. Motive par défis groupe (accountability)

### Stratégies de remboursement (Phase 1)

**Avalanche (Highest Interest First):**
- Payer d'abord la dette avec le taux d'intérêt le plus élevé
- Minimise le total d'intérêts payés
- Économies: parfois 1000+ euros
- Psychologiquement: plus long avant de voir une dette disparaître

**Snowball (Smallest Balance First):**
- Payer d'abord la dette avec le plus petit montant
- Psychologiquement satisfaisant (victoires rapides)
- Crée momentum: une dette éliminée = motivation
- Total d'intérêts légèrement plus élevé qu'avalanche

Exemple concret:
```
Dettes initiales:
- Carte crédit: 3000 EUR, 18% intérêt
- Prêt étudiant: 15000 EUR, 4% intérêt
- Prêt perso: 5000 EUR, 7% intérêt
Budget remboursement: 500 EUR/mois

Stratégie AVALANCHE: Payer d'abord la carte crédit
- Mois 1-6: 500 EUR → Carte crédit
- Carte crédit éliminée en 7 mois
- Ensuite: Prêt perso, puis étudiant
- Total intérêts: ~2500 EUR

Stratégie SNOWBALL: Payer d'abord le prêt perso
- Mois 1-10: 500 EUR → Prêt perso
- Prêt perso éliminé en 11 mois
- Puis carte crédit, puis étudiant
- Total intérêts: ~2750 EUR
- MAIS: Succès visible rapidement = motivation
```

### Défis Groupe (Phase 1)

Les utilisateurs peuvent créer des défis collaboratifs:

**Exemple 1: "Debt Payoff Challenge"**
- 5 amis ensemble se fixent un objectif
- "Rembourser 10 000 EUR en 6 mois"
- Chacun tracking ses propres dettes
- Leaderboard: qui rembourse le plus
- Récompense: dîner ensemble quand terminé

**Exemple 2: "No Spend Week"**
- Groupe défi: zéro dépense superflue pendant 7j
- Notifications quand quelqu'un achète
- Points bonus pour chaque jour sans dépense
- Social: encouragements dans chat

### Fonctionnalités Phase 2 avec IA

**1. Receipt Analyzer (OCR + Claude)**
Utilisateur:
- Prend photo reçu bancaire/lettre créancier
- Upload dans app
- IA (Claude) lit le texte → extrait:
  - Montant exact de la dette
  - Nom du créancier
  - Date d'échéance
  - Auto-création de la dette

Bénéfice: Pas besoin saisir manuellement → moins d'erreurs

**2. Smart Recommendations (Claude)**
Système analyse:
- Historique paiements du user
- Comportement de dépenses
- Taux d'intérêt actuels
- Revenu disponible

Claude propose:
- "Basé sur ton income, tu peux faire 650 EUR/mois"
- "Avalanche est mieux pour toi: économise 800 EUR"
- "Attention: tu dépenses 200 EUR en streaming chaque mois"

**3. Chatbot Financial Advisor**
Utilisateur peut:
- "Je gagne 2500 EUR/mois, comment je peux rembourser 8000 EUR de dettes?"
- "Dois-je payer ma carte crédit ou prêt perso en priorité?"
- "Que signifie APR? Quand je paye des intérêts?"

Claude répond conversationnellement + conseils personnalisés

**4. Predictions (Claude)**
Système calcule:
- "À ce rythme, tu seras debt-free le 15 juin 2026"
- "Si tu augmentes de 100 EUR/mois, tu finiras 4 mois plus tôt"
- "Attention: inflation va augmenter tes taux"

---

## FLOW UTILISATEUR (User Journey)

### Scénario 1: Nouveau Utilisateur (Phase 1)

1. **Inscription**: Email + mot de passe
2. **Dashboard vide**: "Ajoute tes dettes"
3. **Créer Dette 1**:
   - Créancier: "Banque XYZ"
   - Montant original: 5000 EUR
   - Intérêt: 8%
   - Date due: 31/12/2026
   - Type: Prêt personnel

4. **Créer Dettes 2-3**: Carte crédit, prêt étudiant

5. **Voir Dashboard**:
   - Total dettes: 23 000 EUR
   - Intérêt moyen: 7.5%
   - Budget remboursement: 600 EUR/mois

6. **Générer Plan**:
   - Choisir stratégie: "Avalanche"
   - App calcule: "Debt-free en 43 mois"
   - Montre timeline visuelle

7. **Enregistrer Paiement**:
   - "J'ai payé 600 EUR aujourd'hui"
   - Sélectionner quelle dette
   - Progression visuelle: barre de progression

### Scénario 2: Utilisateur avec Défis (Phase 1)

8. **Créer Défi**:
   - Inviter 3 amis
   - Objectif: "Rembourser 5000 EUR ensemble en 3 mois"
   - Chat groupe: encouragements

9. **Leaderboard**:
   - Voir qui rembourse le plus cette semaine
   - Notifications: "Tom vient de rembourser 500 EUR!"
   - Points bonus pour streaks (paiements consécutifs)

### Scénario 3: Avec IA (Phase 2)

10. **Upload Reçu** (optionnel):
    - Photo lettre banque
    - Claude extrait: "Carte crédit, 2000 EUR, 18% intérêt"
    - Auto-crée la dette

11. **Demander Conseil IA**:
    - Chat: "Je gagne 3000 EUR, 2 enfants, comment je paye mes dettes?"
    - Claude: "Considère Snowball plutôt qu'Avalanche pour motivation"
    - Recommande budget: 700 EUR/mois dettes

12. **Voir Prédictions**:
    - Timeline: "Debt-free le 15/09/2027 si tu payes 700 EUR/mois"
    - Graphique: visualiser progression
    - Alert: "Attention: tu as dépensé 450 EUR en restaurants ce mois"

---

## ARCHITECTURE GÉNÉRALE

```
┌─────────────────────────────────────────────────────────┐
│                    BLAZOR WEBASSEMBLY                    │
│              (Frontend - Single Page App)                │
└────────────────────┬────────────────────────────────────┘
                     │ REST API + JWT
┌────────────────────▼────────────────────────────────────┐
│          ASP.NET CORE WEB API (Gateway Pattern)          │
│  - Controllers (CQRS avec MediatR)                       │
│  - Validation, Filters, Error Handling                   │
│  - Optional: IA Integration Layer (Phase 2)              │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│             DOMAIN & APPLICATION LAYERS                  │
│  - Domain: Entities (Debt, User, Plan, Challenge)        │
│  - Use Cases: Commands & Queries (MediatR)               │
│  - Validators, DTOs                                      │
│  - IA Services: Claude/OpenAI API calls (Phase 2)         │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│           INFRASTRUCTURE & PERSISTENCE                   │
│  - EF Core + Repository Pattern                          │
│  - SQL Server Database                                   │
│  - Caching (Redis optional)                              │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────┴───────────────────────┐
         │                                   │
    ┌────▼─────┐         ┌──────▼───┐  ┌───▼──────────┐
    │ SQL DB   │         │ Redis    │  │ Claude/GPT   │
    └──────────┘         └──────────┘  │ (Phase 2)    │
                                       └──────────────┘
```

---

## STRUCTURE DU PROJET

### **PHASE 1: MVP sans IA (Jours 1-5)**
Architecture solide, fonctionalités core, pas de dépendance externe IA

### **PHASE 2: Evolution avec IA (Jours 6-8)**
Intégration optionnelle: Claude API, OpenAI API pour features avancées
(Anthropic SDK pour orchestration)

---

## FONCTIONNALITÉS PAR NIVEAUX

### **NIVEAU 1 - DÉBUTANTS (Tâches "fondamentales")**
MVP sans IA - construire les fondations :

**Objectif:** Permettre aux utilisateurs d'enregistrer et voir leurs dettes

- [ ] **T1.1** Créer 4 entités Domain (User, Debt, Payment, Challenge)
  - User: userId, email, fullName, totalDebt
  - Debt: debtId, userId, creditor, originalAmount, remainingAmount, interestRate, dueDate, type (CREDIT_CARD, PERSONAL_LOAN, etc), status (ACTIVE, PAID_OFF)
  - Payment: paymentId, debtId, amount, paymentDate, notes
  - Challenge: challengeId, title, participants, targetAmount, dueDate

- [ ] **T1.2** Générer migrations EF Core
  - Créer tables: Users, Debts, Payments, Challenges, UserChallenges (junction)
  - Ajouter indexes: userId, debtId
  - Ajouter contraintes: foreign keys, check constraints

- [ ] **T1.3** Implémenter Repository<T> générique
  - Méthodes: GetAll(), GetById(id), Create(entity), Update(entity), Delete(id)
  - LINQ basics: Where, FirstOrDefault, ToList
  - Pas de caching, pas de pagination encore

- [ ] **T1.4** Créer DTOs pour Create/Update Debt
  - CreateDebtDto: creditor, originalAmount, interestRate, dueDate, type
  - UpdateDebtDto: same fields (optional)
  - Mapping simple: Entity ↔ DTO

- [ ] **T1.5** Endpoints CRUD simples
  - GET /api/v1/debts → List all user debts
  - GET /api/v1/debts/{id} → Get one debt
  - POST /api/v1/debts → Create new debt
  - PATCH /api/v1/debts/{id} → Update debt
  - DELETE /api/v1/debts/{id} → Delete debt

- [ ] **T1.6** Composants Blazor basiques
  - DebtList.razor: @foreach debts, afficher dans tableau
  - DebtDetail.razor: formulaire edit debt
  - Pas de styling complexe, Bootstrap basic

- [ ] **T1.7** Validation côté serveur
  - Data Annotations: [Required], [Range], [StringLength]
  - ModelState check dans controller
  - Messages d'erreur simples

- [ ] **T1.8** Tests unitaires simples
  - Test: Create debt with valid data → success
  - Test: Create debt with negative amount → validation error
  - Test: Get debt by id → returns correct debt

**Output attendu:**
- Utilisateur peut créer/lire/modifier ses dettes
- Dashboard affiche liste dettes
- Endpoint API Postman testable
- 3 tests unitaires passent

---

### **NIVEAU 2 - INTERMÉDIAIRES (Tâches "enrichissement")**
Logique métier, plans remboursement, défis groupe (toujours sans IA) :

**Objectif:** Créer des plans de remboursement intelligents et suivi temps réel

- [ ] **T2.1** Plan de remboursement automatique (Avalanche vs Snowball)
  - Service: RemboursementPlanCalculator
  - Avalanche: Sort debts par interestRate DESC → priorité taux élevés
  - Snowball: Sort debts par remainingAmount ASC → priorité petits montants
  - Calcule: mois pour debt-free, total intérêts payés, monthly payment suggeré

- [ ] **T2.2** Système de paiements et suivi progression
  - POST /api/v1/debts/{id}/payments → enregistrer paiement
  - Mettre à jour remainingAmount automatiquement
  - Marquer debt comme PAID_OFF quand remainingAmount = 0
  - Calculer intérêts accrués depuis dernier paiement

- [ ] **T2.3** Gestion des défis groupe collaboratifs
  - POST /api/v1/challenges → créer challenge
  - POST /api/v1/challenges/{id}/join → joindre challenge
  - Tracker progrès collectif (tous les participants)
  - Leaderboard: qui a payé le plus ce mois

- [ ] **T2.4** Pagination et filtrage avancé
  - GET /api/v1/debts?status=ACTIVE&type=CREDIT_CARD&sortBy=dueDate
  - Pagination: limit=10, offset=0
  - Filtres: status, type, minAmount, maxAmount, dueSoon (< 30 days)

- [ ] **T2.5** Queries CQRS pour rapports
  - Query: GetDebtSummary → { totalDebt, totalInterestPaid, averageRate, monthlyPaymentSuggested }
  - Query: GetDebtByMonth → { month, paymentMade, interestPaid, remainingTotal }
  - Dashboard stats: visualiser progression dans le temps

- [ ] **T2.6** Formulaires Blazor avec validation côté client
  - InputNumber pour montants (min=0, max=1000000)
  - InputSelect pour type dette
  - ValidationMessage pour afficher erreurs
  - Submit button disable si form invalid

- [ ] **T2.7** Notification temps réel (SignalR)
  - SignalR Hub: PaymentNotificationHub
  - Quand paiement enregistré, notifier user: "Tu as payé 500 EUR!"
  - Quand défi atteint objectif, congratulations message
  - Chat groupe dans défi (optionnel)

- [ ] **T2.8** Tests d'intégration (5 tests)
  - Test: Create debt + Record payment → remaining decreases
  - Test: Calculate Avalanche plan → correct order
  - Test: Calculate Snowball plan → correct order
  - Test: Get debt summary → correct totals
  - Test: Join challenge → user added to participants

**Output attendu:**
- Utilisateurs voient plans de remboursement (Avalanche vs Snowball)
- Peuvent enregistrer paiements et voir progression
- Peuvent créer/joindre défis groupe
- Dashboard affiche statistiques: total restant, intérêts payés, date debt-free
- Notifications SignalR quand action importante

---

### **NIVEAU 3 - CONFIRMÉS (Tâches "architecture & Phase 2 - IA optionnelle")**
Patterns avancés + intégration IA externe :

**Objectif:** Code robuste, extensible, prêt pour IA

- [ ] **T3.1** MediatR avec Pipeline Behaviors (Validation, Logging)
  - CreateDebtCommand → MediatR handler → Validation pipeline → Logger → Execute
  - Permet d'ajouter features transversales (validation, logging) sans dupliquer code
  - Phase 2: Ajouter behavior pour appels IA

- [ ] **T3.2** Autorisation granulaire (admin, user, financial advisor)
  - [Authorize] sur controllers
  - Policy-based: "CanEditDebt" = user owns debt OR admin
  - FinancialAdvisor role: peut voir dettes des clients (privacy)

- [ ] **T3.3** Caching stratégique (Redis)
  - Cache: DebtSummary pendant 1h (rarement change)
  - Cache: Remboursement plans pendant 24h
  - Cache invalidation: quand paiement enregistré

- [ ] **T3.4** Audit Trail (modifications dettes, paiements)
  - AuditLog table: { entityType, entityId, action, oldValue, newValue, timestamp, userId }
  - Enregistrer: "User X changed Debt Y remaining from 5000 to 4500 on 2026-03-04"
  - Important pour: légal, debugging, trace fraude

- [ ] **T3.5** Event Sourcing basique
  - Domain events: DebtCreated, PaymentRecorded, StrategyChanged
  - Event handlers: publier notifications, mettre à jour cache, déclencher IA (Phase 2)

- [ ] **T3.6** API Versioning (v1, v2 avec IA)
  - v1: endpoints sans IA
  - v2: ajoute /api/v2/analyze-receipt, /api/v2/recommendations
  - Clients peuvent rester sur v1 si IA n'existe pas

- [ ] **T3.7** Rate Limiting et Throttling
  - 1000 requests/hour par user
  - Évite abuses: créer 1000 dettes spam
  - Important quand IA active (contrôle coûts)

- [ ] **T3.8** Tests de charge
  - Simuler: 100 users créent 5 dettes chacun
  - Mesurer: temps réponse, mémoire, CPU
  - Valider: < 200ms median latency

- [ ] **T3.9 - IA FEATURE** Claude API: Analyse texte reçus
  - Utilisateur: upload PDF facture/lettre bancaire
  - OCR → texte brut
  - Claude: "Extract: amount, creditor, interest rate, due date"
  - Retour: structured data → auto-create Debt

- [ ] **T3.10 - IA FEATURE** Recommandations smart (Claude)
  - Claude analyse dettes: "Amount: 5000, Rate: 18%, Monthly budget: 600"
  - Claude retourne: "Use Avalanche (saves 500 EUR) instead of Snowball"
  - Explique reasoning: taux élevé = beaucoup intérêts accrués

- [ ] **T3.11 - IA FEATURE** Prédictions (Claude)
  - Calcule: "À 600 EUR/mois, tu finiras le 15 septembre 2027"
  - Scenarios: "Si tu augmentes à 700 EUR, finiras le 15 mai 2027"
  - Graphique: timeline interactive

**Output attendu:**
- Code extensible: facile ajouter nouvelles features
- Audit complet: tracer tout changement
- Prêt Phase 2: infrastructure IA en place
- Performance validée: < 200ms latency

---

### **NIVEAU 4 - EXPERTS (Tâches "infrastructure & IA orchestration")**
DevOps, déploiement, Semantic Kernel pour orchestration IA :

**Objectif:** Application scalable, resilient, avec IA orchestration avancée

- [ ] **T4.1** Dockeriser l'API (multi-stage build)
  - Stage 1: SDK .NET → compile
  - Stage 2: Runtime → copy DLL → lean image (50MB vs 500MB)
  - Dockerfile: expose port 5000, healthcheck
  - Docker image: debtfreezer-api:latest

- [ ] **T4.2** Dockeriser Blazor WASM (Nginx reverse proxy)
  - Build Blazor WASM → static files (dist/)
  - Nginx config: serve index.html, proxy API calls to backend
  - Port: 80 (frontend), redirected /api/* to backend

- [ ] **T4.3** docker-compose stack
  - Services: api (port 5000), frontend (port 80), sql-server (port 1433), redis (port 6379), nginx (port 80/443)
  - Networks: debtfreezer-net
  - Volumes: sql-server persisted data
  - healthcheck: api /health → docker-compose waits before starting dependents

- [ ] **T4.4** GitHub Actions pipeline
  - Trigger: push to main
  - Jobs: build API, run tests, build Docker image, push to Docker Hub
  - Matrix: test on Linux + Windows
  - Artifact: test reports, coverage

- [ ] **T4.5** Déploiement Azure (App Service)
  - Create Azure App Service (Linux, .NET 8)
  - Deploy Docker image from Docker Hub
  - Create Azure SQL Database (production)
  - Connection string via Azure Key Vault

- [ ] **T4.6** Azure SQL Database + migration
  - Create database: debtfreezer-prod
  - Run EF migrations: dotnet ef database update --environment Production
  - Configure: firewall rules, backup, geo-replication

- [ ] **T4.7** Application Insights et monitoring
  - Enable Azure Monitor: collect logs, metrics, exceptions
  - Custom metrics: API latency, IA API calls, cache hit rate
  - Alerts: if latency > 500ms OR error rate > 1%
  - Dashboard: real-time KPIs

- [ ] **T4.8 - IA ORCHESTRATION** Anthropic SDK + Semantic Kernel
  - Anthropic SDK: wrapper autour Claude API
  - Semantic Kernel: orchestrate workflows
  - Example: User asks "Help me" → SK calls (1) Analyze debts, (2) Generate strategy, (3) Predict timeline
  - Chaining: output of step 1 → input of step 2

- [ ] **T4.9 - IA CHATBOT** Financial advisor conversationnel
  - Endpoint: POST /api/v2/ai/chat
  - Input: { userId, message, conversationHistory }
  - Claude: maintain context, answer financial questions
  - Examples:
    - "Comment je peux rembourser 10K EUR?"
    - "Avalanche ou Snowball pour moi?"
    - "Que faire si je peux pas payer ce mois?"
  - Claude répond avec conseils + lien vers app features

- [ ] **T4.10 - IA ANALYTICS** Pattern recognition
  - Analyse: tous les paiements utilisateur
  - Claude extrait: "Tu payes +2% le dernier jour du mois" OR "Tu dépenses 500 EUR en restaurants"
  - Recommandations: "Augmente ton budget de 50 EUR pour panier moins stressé"
  - Comportement: identifier struggles, suggest tweaks

**Output attendu:**
- API + Frontend docker images: production-grade
- Automated CI/CD: push code → tests → deploy in 10 min
- Monitoring en place: on voit immédiatement si serveur down/lent
- IA orchestration: multiple Claude calls chainées seamlessly
- Chatbot: conversationnel + helpful financial advice

---

## DISTRIBUTION PAR JOUR

### **JOUR 1 - Planning & Setup**

- Réunion lancement (vision, MVP vs Phase 2)
- Répartition groupes par niveau
- Setup: repos Git, Visual Studio, SQL Server

- N1: Solution structure + DbContext
- N2: Design entités + diagram
- N3: Plan MediatR + CQRS
- N4: docker-compose.yml prép

---

### **JOUR 2 - Backend Core (MVP)**

- **N1:** Crée 4 entités + DbContext
- **N2:** DTOs + mapping strategy
- **N3:** MediatR setup + handlers stubs
- **N4:** Dockerfile + networking

- **N1:** Migrations + seed data + Repository générique
- **N2:** Implémente plans remboursement (avalanche/snowball)
- **N3:** Validation pipeline behaviors
- **N4:** Docker-compose local test

---

### **JOUR 3 - API & Frontend (MVP)**

- **N1:** Controllers CRUD endpoints
- **N2:** Logique paiements + suivi
- **N3:** Query handlers + caching
- **N4:** GitHub Actions workflow

- **N1:** Blazor components (DebtList, Detail)
- **N2:** Forms paiements + validation
- **N3:** SignalR notifications
- **N4:** Image build & push

---

### **JOUR 4 - Features Avancees & Tests**

- **N1:** Tests unitaires service (3 tests)
- **N2:** Tests intégration API (5 tests)
- **N3:** Performance testing
- **N4:** Azure provisioning

- **N1:** Fix bugs + code review
- **N2:** Défis groupe implémentation
- **N3:** Audit Trail implementation
- **N4:** Déploiement test env

---

### **JOUR 5 - MVP Finalisé**

- **N1:** Coverage > 60%, validation finale
- **N2:** End-to-end workflow (dette → paiements)
- **N3:** Versioning API
- **N4:** Production readiness checklist

- Tous: Démo MVP sans IA
- Tous: Documentation Phase 1

---

### **JOUR 6 - Phase 2 Optionnelle: IA Integration (Niveau 3&4)**

- **N3 & N4:** Intégration Claude API
- **N3 & N4:** Endpoint: /api/v2/analyze-receipt (OCR → extraction IA)
- **N3 & N4:** Endpoint: /api/v2/recommend-strategy (analyse dettes → recommandations)
- **N3 & N4:** Tests IA features

- **N4:** Anthropic SDK setup + Semantic Kernel
- **N4:** Planifier orchestration IA

---

### **JOUR 7 - Chatbot & Analytics IA**

- **N3:** Prédictions: endpoint debt-free date (Claude)
- **N4:** Chatbot financial advisor (SignalR + Claude)
- **N4:** Semantic Kernel: chaîner appels IA complexes

- **N4:** Monitoring IA (coûts tokens, latence)
- Tous: Tests E2E avec IA

---

### **JOUR 8 - Demo Complète & Documentation**

- Démo Phase 1 + Phase 2 (IA optionnelle)
- Documentation architecture (IA layer)
- Documentation API (endpoints IA)
- Rétrospective & celebration

---

## ARCHITECTURE TECHNIQUE

### **Entités métier (Domain Model)**

Structures principales :
- User: utilisateur avec identité et historique
- Debt: dette avec montants, taux, dates
- Payment: transactions de remboursement
- RemboursementPlan: stratégies (avalanche/snowball)
- Challenge: défis groupe collaboratifs
- AIRecommendation: suggestions (Phase 2)
- ReceiptAnalysis: extraction IA de documents

### **Endpoints API par niveau & phase**

**NIVEAU 1 (MVP - Phase 1):**
- Dettes: GET all, GET by id, POST create, PATCH update, DELETE
- Basique: lecture/écriture dettes

**NIVEAU 2 (MVP + Business Logic - Phase 1):**
- Paiements: enregistrer, historique
- Plans: calculer stratégies (avalanche/snowball)
- Défis: créer/consulter défis groupe
- Filtrage et rapports simples

**NIVEAU 3 (Advanced + IA - Phase 2):**
- API v2 avec caching
- Receipt analyzer (OCR + extraction IA)
- Audit trail
- Recommandations (Claude)
- Prédictions (debt-free date)

**NIVEAU 4 (DevOps + IA Orchestration - Phase 2):**
- Health checks, métriques
- Chatbot financial advisor
- Analytics comportementales (IA)
- Admin endpoints

---

## LIVRABLES PAR NIVEAU

| Niveau | Git Branch | Docker | Tests | Documentation |
|--------|-----------|--------|-------|----------------|
| **N1** | `feature/core-crud` | NON | 3 tests (Unit) | API Postman collection |
| **N2** | `feature/debt-management` | API seulement | 8 tests (Unit+Int) | User Stories + Diagrams |
| **N3** | `feature/ia-integration` | OUI | 15+ tests | Architecture Decision Records |
| **N4** | `main` (merged) | OUI Complet | E2E tested | Deployment Runbook |

---

## MAPPING MODULES → TÂCHES

| Module | Tâches Correspondantes |
|--------|----------------------|
| M1 (Algo) | T1.7 (validation), T2.4 (algorithmes avalanche/snowball) |
| M2 (Git) | Tout (branching strategy Phase 1 → Phase 2) |
| M3 (C# basique) | T1.1, T1.2, T1.4, T1.6 |
| M4 (POO) | T1.1, T2.1, T3.1 (design patterns) |
| M5 (SQL) | T1.2, T3.4 (audit), T4.6 |
| M6 (EF Core) | T1.3, T2.2, T3.2 (relationships) |
| M7 (Web) | T1.6, T2.6, T2.7 (Blazor) |
| M8 (MVC) | T1.5, T2.5 (controllers) |
| M9 (API REST) | T1.5, T2.3, T3.6 (versioning) |
| M10 (Blazor) | T1.6, T2.6, T2.7 (components) |
| M11 (Design Patterns) | T3.1, T3.5 (patterns) |
| M12 (Tests) | T1.8, T2.8, T3.8 (all tests) |
| M13 (Docker) | T4.1, T4.2, T4.3 |
| M14 (CI/CD) | T4.4, T4.5 |
| M15 (Azure) | T4.6, T4.7 |
| M17 (Security) | T2.2, T3.2 (auth) |
| M18 (IA) | **T3.9-11, T4.8-10** (Claude/OpenAI API calls) |

---

## ÉLÉMENTS À TIROIR (Difficulty Slider)

## ÉLÉMENTS À TIROIR (Difficulty Slider)

### **Pour SIMPLIFIER (N1 réussit - Phase 1 only)**

Niveau 1 implémente:
- CRUD basique sans async complexe
- Entités simples (Debt avec champs essentiels)
- Pas de calculs automatiques de plans
- Repositories basiques sans caching
- Validation Data Annotations simple

### **Pour ENRICHIR (N2 - Phase 1 complete)**

Niveau 2 ajoute:
- Stratégies avalanche vs snowball (algorithmes)
- Notifications SignalR pour rappels paiements
- Filtrage et recherche
- Plans de remboursement calculés
- Défis groupe collaboratifs

### **Pour CHALLENGER (N3/N4 - Phase 2 avec IA)**

Niveau 3 & 4 implémentent:
- Claude API: Analyse reçus (OCR + extraction montants)
- Recommandations smart (stratégies optimisées)
- Prédictions debt-free date
- Semantic Kernel pour orchestration IA
- Chatbot financial advisor conversationnel

---

## RESSOURCES PAR NIVEAU

### **N1 Débutants**
- Microsoft Learn: EF Core Basics
- C# documentation
- LINQ tutorials

### **N2 Intermédiaires**
- Clean Architecture
- Domain-Driven Design
- CQRS patterns

### **N3 Confirmés**
- MediatR documentation
- Polly resilience
- Claude API docs
- OpenAI API docs

### **N4 Experts**
- Docker best practices
- Azure DevOps
- Anthropic SDK & Semantic Kernel docs
- Claude API documentation
