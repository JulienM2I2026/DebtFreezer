import { Mail, Lock, User, ArrowRight, BadgeEuro } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useNavigate, Link } from "react-router-dom";
import { useState } from "react";
import { RegisterDto } from "@/dtos/RegisterDto";
import debtFreezerLogo from "@/assets/debtfreezer-logo-premium.png";

const Signup = () => {
  const navigate = useNavigate();

  const [form, setForm] = useState<RegisterDto>({
    email: "",
    fullName: "",
    password: "",
    monthlyRepaymentBudget: null,
    repaymentStrategy: 1,
  });

  async function createAccount(dto: RegisterDto) {
    const response = await fetch("http://localhost:5099/api/Auth/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(dto),
    });

    const data = await response.json();

    if (!response.ok) {
      console.log("Failed to create account", data);
      return;
    }

    console.log(data);
    navigate("/login");
    return data;
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    createAccount(form);
  };

  return (
    <div className="min-h-screen flex">
      <div className="hidden lg:flex lg:w-1/2 gradient-hero items-center justify-center p-12 relative overflow-hidden">
        <div className="absolute bottom-20 right-1/4 w-72 h-72 rounded-full bg-success/20 blur-[120px]" />

        <div className="relative z-10 flex flex-col items-center text-center max-w-xl">
          <img
            src={debtFreezerLogo}
            alt="Logo DebtFreezer"
            className="h-64 w-64 object-contain mb-6 drop-shadow-[0_0_40px_rgba(59,130,246,0.45)]"
          />

            <h2 className="font-display text-5xl leading-tight font-bold text-primary-foreground mb-4 max-w-lg mx-auto">
              Prêt(e) à geler
              <br />
              tes dettes ?
            </h2>

          <p className="text-primary-foreground/75 text-xl mb-8 max-w-md">
            Crée ton espace et démarre simplement.
          </p>

    <div className="flex flex-wrap items-center justify-center gap-3 max-w-md">
      <span className="rounded-full bg-white/10 px-4 py-2 text-sm text-primary-foreground/90">
        Plan clair
      </span>
      <span className="rounded-full bg-white/10 px-4 py-2 text-sm text-primary-foreground/90">
        Stratégie adaptée
      </span>
      <span className="rounded-full bg-white/10 px-4 py-2 text-sm text-primary-foreground/90">
        Progression visible
      </span>
    </div>
  </div>
</div>

      <div className="flex-1 flex items-center justify-center p-8">
        <div className="w-full max-w-md">
          <div className="flex items-center gap-3 mb-8 lg:hidden">
            <img
              src={debtFreezerLogo}
              alt="Logo DebtFreezer"
              className="h-10 w-10 object-contain"
            />
            <span className="font-display text-xl font-bold">DebtFreezer</span>
          </div>

          <h1 className="font-display text-3xl font-bold text-foreground mb-2">
            Créer un compte
          </h1>

          <p className="text-muted-foreground mb-8">
            Fais le premier pas vers ta liberté financière.
          </p>

          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="name" className="text-sm font-semibold">
                Prénom ou pseudo
              </Label>
              <p className="text-xs text-muted-foreground">
                Le nom affiché dans ton espace personnel.
              </p>
              <div className="relative">
                <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="name"
                  placeholder="Ex. MonPrénom"
                  className="pl-10 h-12 rounded-xl"
                  type="text"
                  value={form.fullName}
                  onChange={(e) =>
                    setForm({ ...form, fullName: e.target.value })
                  }
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="email" className="text-sm font-semibold">
                Adresse e-mail
              </Label>
              <p className="text-xs text-muted-foreground">
                Pour sécuriser et retrouver ton compte.
              </p>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="email"
                  placeholder="exemple@email.com"
                  className="pl-10 h-12 rounded-xl"
                  type="email"
                  value={form.email}
                  onChange={(e) =>
                    setForm({ ...form, email: e.target.value })
                  }
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label
                htmlFor="MonthlyRepaymentBudget"
                className="text-sm font-semibold"
              >
                Budget mensuel de remboursement
              </Label>
              <p className="text-xs text-muted-foreground">
                Le montant que tu peux consacrer chaque mois.
              </p>
              <div className="relative">
                <BadgeEuro className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="MonthlyRepaymentBudget"
                  placeholder="Ex. 600"
                  className="pl-10 h-12 rounded-xl"
                  type="number"
                  value={form.monthlyRepaymentBudget ?? ""}
                  onChange={(e) =>
                    setForm({
                      ...form,
                      monthlyRepaymentBudget:
                        e.target.value === "" ? null : Number(e.target.value),
                    })
                  }
                />
              </div>
            </div>

            <div className="space-y-3">
              <div>
                <Label className="text-sm font-semibold">
                  Choisis ta stratégie
                </Label>
                <p className="text-xs text-muted-foreground mt-1">
                  Tu pourras la modifier plus tard.
                </p>
              </div>

              <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <button
                  type="button"
                  onClick={() => setForm({ ...form, repaymentStrategy: 1 })}
                  className={`rounded-2xl border p-4 text-left transition-all ${
                    form.repaymentStrategy === 1
                      ? "border-primary bg-primary/5 shadow-sm ring-1 ring-primary/20"
                      : "border-border hover:border-primary/40 hover:bg-muted/40"
                  }`}
                >
                  <div className="flex items-center justify-between mb-2">
                    <span className="font-semibold text-foreground">
                      Snowball
                    </span>
                    {form.repaymentStrategy === 1 && (
                      <span className="text-xs font-medium text-primary bg-primary/10 px-2 py-1 rounded-full">
                        Sélectionné
                      </span>
                    )}
                  </div>

                  <p className="text-sm font-medium text-foreground mb-1">
                    Petites dettes d’abord
                  </p>
                  <p className="text-sm text-muted-foreground">
                    Idéal pour rester motivé(e) grâce à des résultats rapides.
                  </p>
                </button>

                <button
                  type="button"
                  onClick={() => setForm({ ...form, repaymentStrategy: 2 })}
                  className={`rounded-2xl border p-4 text-left transition-all ${
                    form.repaymentStrategy === 2
                      ? "border-primary bg-primary/5 shadow-sm ring-1 ring-primary/20"
                      : "border-border hover:border-primary/40 hover:bg-muted/40"
                  }`}
                >
                  <div className="flex items-center justify-between mb-2">
                    <span className="font-semibold text-foreground">
                      Avalanche
                    </span>
                    {form.repaymentStrategy === 2 && (
                      <span className="text-xs font-medium text-primary bg-primary/10 px-2 py-1 rounded-full">
                        Sélectionné
                      </span>
                    )}
                  </div>

                  <p className="text-sm font-medium text-foreground mb-1">
                    Taux élevés d’abord
                  </p>
                  <p className="text-sm text-muted-foreground">
                    Idéal pour réduire le coût total des intérêts.
                  </p>
                </button>
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="password" className="text-sm font-semibold">
                Mot de passe
              </Label>
              <p className="text-xs text-muted-foreground">
                8 caractères minimum recommandés.
              </p>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="password"
                  placeholder="Mon mot de passe"
                  className="pl-10 h-12 rounded-xl"
                  type="password"
                  value={form.password}
                  onChange={(e) =>
                    setForm({ ...form, password: e.target.value })
                  }
                />
              </div>
            </div>

            <Button
              type="submit"
              className="w-full h-12 rounded-xl gradient-primary border-0 text-base font-semibold shadow-md hover:opacity-95"
            >
              Créer mon compte
              <ArrowRight className="ml-2 h-4 w-4" />
            </Button>
          </form>

          <p className="text-center text-sm text-muted-foreground mt-6">
            Déjà un compte ?{" "}
            <Link
              to="/login"
              className="text-primary font-semibold hover:underline"
            >
              Se connecter
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Signup;