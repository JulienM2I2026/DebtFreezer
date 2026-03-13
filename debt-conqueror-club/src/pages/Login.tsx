import { Mail, Lock, ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useNavigate, Link } from "react-router-dom";
import { useState } from "react";
import { LoginDto } from "@/dtos/LoginDto";
import debtFreezerLogo from "@/assets/debtfreezer-logo-premium.png";

const Login = () => {
  const navigate = useNavigate();

  const [form, setForm] = useState<LoginDto>({
    email: "",
    password: "",
  });

  async function login(dto: LoginDto) {
    const response = await fetch("http://localhost:5099/api/Auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(dto),
    });

    const data = await response.json();

    if (!response.ok) {
      console.log("Échec de la connexion", data);
      return;
    }

    if (data?.accessToken) {
      localStorage.setItem("token", data.accessToken);
    }

    navigate("/dashboard");
    return data;
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    login(form);
  };

  return (
    <div className="min-h-screen flex">
      <div className="hidden lg:flex lg:w-1/2 gradient-hero items-center justify-center p-12 relative overflow-hidden">
        <div className="absolute top-20 left-1/4 w-72 h-72 rounded-full bg-primary/30 blur-[120px]" />

        <div className="relative z-10 flex flex-col items-center text-center max-w-xl">
          <img
            src={debtFreezerLogo}
            alt="Logo DebtFreezer"
            className="h-64 w-64 object-contain mb-6 drop-shadow-[0_0_35px_rgba(59,130,246,0.45)]"
          />

          <h2 className="font-display text-5xl leading-tight font-bold text-primary-foreground mb-4 max-w-lg">
            Prêt(e) à geler tes dettes ?
          </h2>

          <p className="text-primary-foreground/75 text-xl mb-8 max-w-md">
            Chaque connexion te rapproche d’une vie financière plus légère.
          </p>

          <div className="flex flex-wrap items-center justify-center gap-3 max-w-md">
            <span className="rounded-full bg-white/10 px-4 py-2 text-sm text-primary-foreground/90">
              Suivi clair
            </span>
            <span className="rounded-full bg-white/10 px-4 py-2 text-sm text-primary-foreground/90">
              Objectifs visibles
            </span>
            <span className="rounded-full bg-white/10 px-4 py-2 text-sm text-primary-foreground/90">
              Progression motivante
            </span>
          </div>
    </div>
  </div>
      <div className="flex-1 flex items-center justify-center p-8">
        <div className="w-full max-w-lg">
          <h1 className="font-display text-3xl font-bold text-foreground mb-2">
            Connexion à DebtFreezer
          </h1>

          <p className="text-muted-foreground mb-8">
            Retrouve ton espace et poursuis ton plan vers la liberté financière.
          </p>

          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="email" className="text-sm font-semibold">
                Adresse e-mail
              </Label>
              <p className="text-xs text-muted-foreground">
                Utilise l’adresse liée à ton compte.
              </p>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="email"
                  type="email"
                  placeholder="exemple@email.com"
                  className="pl-10 h-12 rounded-xl"
                  value={form.email}
                  onChange={(e) =>
                    setForm({ ...form, email: e.target.value })
                  }
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="password" className="text-sm font-semibold">
                Mot de passe
              </Label>
              <p className="text-xs text-muted-foreground">
                Entre ton mot de passe pour accéder à ton espace.
              </p>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="password"
                  type="password"
                  placeholder="••••••••"
                  className="pl-10 h-12 rounded-xl"
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
              J’entre dans mon espace
              <ArrowRight className="ml-2 h-4 w-4" />
            </Button>
          </form>

          <p className="text-center text-sm text-muted-foreground mt-6">
            Nouveau ici ?{" "}
            <Link
              to="/signup"
              className="text-primary font-semibold hover:underline"
            >
              Ouvre ton espace
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login;