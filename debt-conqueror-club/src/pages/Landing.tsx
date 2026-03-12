import {
  TrendingDown,
  Users,
  CreditCard,
  BarChart3,
  Target,
  CircleDollarSign,
  ArrowRight,
  Shield,
  Zap,
  Trophy,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import debtFreezerLogo from "@/assets/debtfreezer-logo-premium.png";

const Landing = () => {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-background">
      {/* Nav */}
      <nav className="fixed top-0 inset-x-0 z-50 border-b border-white/10 bg-[#16204a]/75 backdrop-blur-md">
        <div className="max-w-7xl mx-auto flex items-center justify-between px-6 h-16">
          <div className="flex items-center gap-3">
            <img
              src={debtFreezerLogo}
              alt="Logo DebtFreezer"
              className="h-10 w-10 object-contain"
            />
            <span className="font-display text-xl font-bold text-white">
              DebtFreezer
            </span>
          </div>

          <div className="hidden md:flex items-center gap-8 text-sm text-white/70">
            <a href="#features" className="hover:text-foreground transition-colors">
              Fonctionnalités
            </a>
            <a href="#how" className="hover:text-foreground transition-colors">
              Comment ça marche
            </a>
            <a href="#social" className="hover:text-foreground transition-colors">
              Défis
            </a>
          </div>

          <div className="flex items-center gap-3">
              <Button
                variant="ghost"
                size="sm"
                className="text-white/80 hover:text-white hover:bg-white/10"
                onClick={() => navigate("/login")}
              >
                Connexion
              </Button>
              <Button
                size="sm"
                className="bg-gradient-to-r from-indigo-500 to-blue-500 border-0 rounded-full px-5 hover:opacity-95"
                onClick={() => navigate("/signup")}
              >
                Commencer
              </Button>
          </div>
        </div>
      </nav>

      {/* Hero */}
      <section className="gradient-hero pt-32 pb-24 px-6 relative overflow-hidden">
        <div className="absolute inset-0 opacity-30">
          <div className="absolute top-16 left-1/4 w-72 h-72 rounded-full bg-primary blur-[120px]" />
          <div className="absolute top-16 left-1/4 w-72 h-72 rounded-full bg-blue-500/20 blur-[120px]" />
        </div>

        <div className="max-w-6xl mx-auto relative z-10 grid lg:grid-cols-2 gap-12 items-center">
          <div className="text-center lg:text-left">
            <div className="inline-flex items-center gap-2 bg-primary/10 border border-primary/20 rounded-full px-4 py-1.5 mb-8 text-sm text-primary-foreground/85">
              <Zap className="h-3.5 w-3.5" />
              <span>Une approche plus intelligente du remboursement</span>
            </div>

            <h1 className="font-display text-5xl md:text-6xl xl:text-7xl font-bold text-primary-foreground leading-tight mb-6">
              Gèle tes dettes.
              <br />
              <span className="bg-gradient-to-r from-blue-400 to-indigo-500 bg-clip-text text-transparent">
                Libère ton avenir.
              </span>
            </h1>

            <p className="text-lg md:text-xl text-primary-foreground/70 max-w-2xl mb-10 leading-relaxed mx-auto lg:mx-0">
              Suis tes dettes, compare tes stratégies de remboursement et avance
              avec une vision claire de tes objectifs financiers.
            </p>

            <div className="flex flex-col sm:flex-row items-center lg:items-start justify-center lg:justify-start gap-4 mb-14">
              <Button
                size="lg"
                className="gradient-primary border-0 text-lg px-8 h-12"
                onClick={() => navigate("/signup")}
              >
                Créer mon compte
                <ArrowRight className="ml-2 h-5 w-5" />
              </Button>

              <Button
                size="lg"
                variant="outline"
                className="text-base px-8 h-12 rounded-xl border border-white/15 bg-white/5 text-primary-foreground hover:bg-white/10"
                onClick={() => navigate("/login")}
              >
                Me connecter
              </Button>
            </div>

            <div className="grid grid-cols-3 gap-6 max-w-lg mx-auto lg:mx-0">
              {[
                { label: "Suivi", value: "Simple" },
                { label: "Stratégies", value: "2" },
                { label: "Objectif", value: "Clair" },
              ].map((stat) => (
                <div key={stat.label} className="text-center lg:text-left">
                  <div className="text-xl md:text-2xl font-display font-bold text-primary-foreground">
                    {stat.value}
                  </div>
                  <div className="text-md text-primary-foreground/60">
                    {stat.label}
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="hidden lg:flex justify-center">
            <div className="relative flex items-center justify-center">
              <div className="absolute inset-0 rounded-full bg-primary/15 blur-[80px]" />
              <img
                src={debtFreezerLogo}
                alt="Logo DebtFreezer"
                className="relative z-10 h-100 w-100 object-contain drop-shadow-[0_0_40px_rgba(59,130,246,0.45)]"
              />
            </div>
          </div>
        </div>
      </section>

      {/* Features */}
      <section id="features" className="py-24 px-6">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-16">
            <h2 className="font-display text-3xl md:text-4xl font-bold text-foreground mb-4">
              Tout ce qu’il te faut pour avancer vers le zéro dette
            </h2>
            <p className="text-muted-foreground text-lg max-w-xl mx-auto">
              Des outils utiles, une lecture claire de ta progression et une
              expérience motivante au même endroit.
            </p>
          </div>

          <div className="grid md:grid-cols-3 gap-6">
            {[
              {
                icon: CreditCard,
                title: "Suivi des dettes",
                desc: "Gère cartes, prêts et autres remboursements dans un espace unique avec une progression visible.",
              },
              {
                icon: BarChart3,
                title: "Simulateur de stratégie",
                desc: "Compare Avalanche et Snowball avec des projections simples et parlantes.",
              },
              {
                icon: Target,
                title: "Objectifs de paiement",
                desc: "Définis des étapes, suis tes efforts et visualise chaque avancée.",
              },
              {
                icon: Users,
                title: "Défis de groupe",
                desc: "Motive-toi avec des proches, partage tes progrès et avance ensemble.",
              },
              {
                icon: TrendingDown,
                title: "Analyse claire",
                desc: "Visualise ton évolution, ton rythme de remboursement et tes gains potentiels.",
              },
              {
                icon: Shield,
                title: "Confidentialité d’abord",
                desc: "Tes données financières restent protégées dans un environnement sécurisé.",
              },
            ].map((f) => (
              <div
                key={f.title}
                className="group bg-card rounded-xl p-6 shadow-card hover:shadow-elevated transition-all duration-300 border border-border hover:border-primary/20"
              >
                <div className="w-12 h-12 rounded-lg gradient-primary flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                  <f.icon className="h-6 w-6 text-primary-foreground" />
                </div>
                <h3 className="font-display text-lg font-semibold text-card-foreground mb-2">
                  {f.title}
                </h3>
                <p className="text-muted-foreground text-sm leading-relaxed">
                  {f.desc}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* How it works */}
      <section id="how" className="py-24 px-6 bg-muted/50">
        <div className="max-w-4xl mx-auto text-center">
          <h2 className="font-display text-3xl md:text-4xl font-bold text-foreground mb-4">
            Comment DebtFreezer fonctionne
          </h2>
          <p className="text-muted-foreground text-lg mb-16">
            Trois étapes pour reprendre le contrôle.
          </p>

          <div className="grid md:grid-cols-3 gap-10">
            {[
              {
                step: "01",
                title: "Ajoute tes dettes",
                desc: "Renseigne tes cartes, prêts et soldes. L’application structure le tout pour toi.",
              },
              {
                step: "02",
                title: "Choisis une stratégie",
                desc: "Compare Avalanche et Snowball et choisis l’approche qui te convient.",
              },
              {
                step: "03",
                title: "Suis et célèbre",
                desc: "Enregistre tes paiements, visualise ta progression et valorise chaque étape.",
              },
            ].map((s) => (
              <div key={s.step} className="relative">
                <div className="text-6xl font-display font-bold text-primary/10 mb-4">
                  {s.step}
                </div>
                <h3 className="font-display text-xl font-semibold text-foreground mb-2">
                  {s.title}
                </h3>
                <p className="text-muted-foreground text-sm">{s.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Social proof / Challenges */}
      <section id="social" className="py-24 px-6">
        <div className="max-w-5xl mx-auto text-center">
          <div className="inline-flex items-center gap-2 bg-success/10 border border-success/20 rounded-full px-4 py-1.5 mb-6 text-sm text-success">
            <Trophy className="h-3.5 w-3.5" />
            <span>Motivation collective</span>
          </div>

          <h2 className="font-display text-3xl md:text-4xl font-bold text-foreground mb-4">
            Avance ensemble
          </h2>

          <p className="text-muted-foreground text-lg max-w-xl mx-auto mb-12">
            Crée des défis, motive-toi avec d’autres personnes et célèbre chaque
            étape vers une situation financière plus saine.
          </p>

          <Button
            size="lg"
            className="gradient-primary border-0"
            onClick={() => navigate("/signup")}
          >
            Rejoindre l’expérience
            <ArrowRight className="ml-2 h-5 w-5" />
          </Button>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t py-8 px-6">
        <div className="max-w-6xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-2">
            <img
              src={debtFreezerLogo}
              alt="Logo DebtFreezer"
              className="h-6 w-6 object-contain"
            />
            <span className="font-display font-semibold text-foreground">
              DebtFreezer
            </span>
          </div>
          <p className="text-sm text-muted-foreground">
            @2026 TP Final - POEC Developpeur .NET by DebtFreezer by Julien N., Essy B. & Myriam M.
          </p>
        </div>
      </footer>
    </div>
  );
};

export default Landing;