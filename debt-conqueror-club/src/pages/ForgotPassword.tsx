import { useState } from "react";
import { Mail, ArrowRight, ArrowLeft, Copy } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Link } from "react-router-dom";
import { toast } from "sonner";

const ForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [resetToken, setResetToken] = useState<string | null>(null);
  const [message, setMessage] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email) return;
    setLoading(true);

    try {
      const response = await fetch("http://localhost:5099/api/v1/Auth/forgot-password", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });

      const data = await response.json();

      if (!response.ok) {
        toast.error(data?.message ?? "Une erreur est survenue.");
        return;
      }

      setMessage(data.message);
      if (data.resetToken) {
        setResetToken(data.resetToken);
      }
    } catch {
      toast.error("Impossible de contacter le serveur.");
    } finally {
      setLoading(false);
    }
  };

  const copyToken = () => {
    if (resetToken) {
      navigator.clipboard.writeText(resetToken);
      toast.success("Token copié !");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-8 bg-background">
      <div className="w-full max-w-md space-y-6">
        <div>
          <h1 className="font-display text-3xl font-bold text-foreground mb-2">
            Mot de passe oublié
          </h1>
          <p className="text-muted-foreground">
            Saisis ton adresse e-mail pour recevoir un token de réinitialisation.
          </p>
        </div>

        {!resetToken ? (
          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="email" className="text-sm font-semibold">
                Adresse e-mail
              </Label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  id="email"
                  type="email"
                  placeholder="exemple@email.com"
                  className="pl-10 h-12 rounded-xl"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
            </div>

            <Button
              type="submit"
              disabled={loading || !email}
              className="w-full h-12 rounded-xl gradient-primary border-0 text-base font-semibold"
            >
              {loading ? "Envoi en cours…" : "Envoyer"}
              {!loading && <ArrowRight className="ml-2 h-4 w-4" />}
            </Button>
          </form>
        ) : (
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>

            <div className="rounded-xl border bg-muted/30 p-4 space-y-2">
              <p className="text-xs text-muted-foreground font-medium">
                Token de réinitialisation (dev)
              </p>
              <div className="flex items-center gap-2">
                <code className="flex-1 text-xs break-all font-mono text-card-foreground">
                  {resetToken}
                </code>
                <Button variant="ghost" size="icon" onClick={copyToken}>
                  <Copy className="h-4 w-4" />
                </Button>
              </div>
            </div>

            <Link to={`/reset-password?token=${resetToken}`}>
              <Button className="w-full h-12 rounded-xl gradient-primary border-0 text-base font-semibold">
                Réinitialiser mon mot de passe
                <ArrowRight className="ml-2 h-4 w-4" />
              </Button>
            </Link>
          </div>
        )}

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

export default ForgotPassword;
