import { useState } from "react";
import { Lock, ArrowRight, ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { toast } from "sonner";

const ResetPassword = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const [token, setToken] = useState(searchParams.get("token") ?? "");
  const [newPassword, setNewPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token || !newPassword) return;

    if (newPassword !== confirm) {
      toast.error("Les mots de passe ne correspondent pas.");
      return;
    }

    if (newPassword.length < 6) {
      toast.error("Le mot de passe doit contenir au moins 6 caractères.");
      return;
    }

    setLoading(true);
    try {
      const response = await fetch("http://localhost:5099/api/v1/Auth/reset-password", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ token, newPassword }),
      });

      const data = await response.json();

      if (!response.ok) {
        toast.error(data?.message ?? "Une erreur est survenue.");
        return;
      }

      toast.success("Mot de passe réinitialisé ! Tu peux te connecter.");
      navigate("/login");
    } catch {
      toast.error("Impossible de contacter le serveur.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-8 bg-background">
      <div className="w-full max-w-md space-y-6">
        <div>
          <h1 className="font-display text-3xl font-bold text-foreground mb-2">
            Nouveau mot de passe
          </h1>
          <p className="text-muted-foreground">
            Saisis ton token de réinitialisation et choisis un nouveau mot de passe.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <div className="space-y-2">
            <Label htmlFor="token" className="text-sm font-semibold">
              Token de réinitialisation
            </Label>
            <Input
              id="token"
              type="text"
              placeholder="Colle ton token ici"
              className="h-12 rounded-xl font-mono text-sm"
              value={token}
              onChange={(e) => setToken(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="newPassword" className="text-sm font-semibold">
              Nouveau mot de passe
            </Label>
            <div className="relative">
              <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                id="newPassword"
                type="password"
                placeholder="••••••••"
                className="pl-10 h-12 rounded-xl"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="confirm" className="text-sm font-semibold">
              Confirmer le mot de passe
            </Label>
            <div className="relative">
              <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                id="confirm"
                type="password"
                placeholder="••••••••"
                className="pl-10 h-12 rounded-xl"
                value={confirm}
                onChange={(e) => setConfirm(e.target.value)}
              />
            </div>
          </div>

          <Button
            type="submit"
            disabled={loading || !token || !newPassword || !confirm}
            className="w-full h-12 rounded-xl gradient-primary border-0 text-base font-semibold"
          >
            {loading ? "Réinitialisation…" : "Confirmer"}
            {!loading && <ArrowRight className="ml-2 h-4 w-4" />}
          </Button>
        </form>

        <p className="text-center text-sm text-muted-foreground">
          <Link to="/login" className="text-primary font-semibold hover:underline flex items-center justify-center gap-1">
            <ArrowLeft className="h-3.5 w-3.5" />
            Retour à la connexion
          </Link>
        </p>
      </div>
    </div>
  );
};

export default ResetPassword;
