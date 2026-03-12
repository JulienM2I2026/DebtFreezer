import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";
import {
  User,
  Mail,
  Bell,
  Shield,
  Trophy,
  Flame,
  Wallet,
  Target,
  Users,
  CalendarDays,
} from "lucide-react";

const Profile = () => {
  return (
    <div className="space-y-6 max-w-5xl mx-auto animate-fade-in pb-8">
      <div className="space-y-1">
        <h1 className="font-display text-3xl font-bold text-foreground">
          Profil utilisateur
        </h1>
        <p className="text-muted-foreground">
          Gère tes informations, tes préférences et ton suivi dans DebtFreezer.
        </p>
      </div>

      {/* KPI row */}
      <div className="grid md:grid-cols-4 gap-4">
        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center mb-3">
            <Wallet className="h-5 w-5 text-primary" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Dette suivie</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            $23 000
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-success/10 flex items-center justify-center mb-3">
            <Flame className="h-5 w-5 text-success" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Streak actif</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            14 jours
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-warning/10 flex items-center justify-center mb-3">
            <Trophy className="h-5 w-5 text-warning" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Badges obtenus</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            3
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-info/10 flex items-center justify-center mb-3">
            <Users className="h-5 w-5 text-info" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Défis actifs</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            2
          </p>
        </Card>
      </div>

      <div className="grid lg:grid-cols-3 gap-6">
        {/* Main profile card */}
        <div className="lg:col-span-2">
          <Card className="p-6 shadow-card rounded-2xl border bg-card/90">
            <div className="flex items-start gap-4 mb-6">
              <div className="w-20 h-20 rounded-full gradient-primary flex items-center justify-center text-2xl font-bold text-primary-foreground shrink-0">
                J
              </div>

              <div className="flex-1">
                <h2 className="font-display text-2xl font-semibold text-card-foreground">
                  John Doe
                </h2>
                <p className="text-sm text-muted-foreground mb-3">
                  john@example.com
                </p>

                <div className="flex flex-wrap gap-3">
                  <Badge className="gradient-success border-0 flex items-center gap-1 rounded-full px-3 py-1">
                    <Flame className="h-3 w-3" />
                    14 jours de régularité
                  </Badge>

                  <Badge
                    variant="outline"
                    className="flex items-center gap-1 rounded-full px-3 py-1"
                  >
                    <Trophy className="h-3 w-3" /> 3 badges débloqués
                  </Badge>

                  <Badge
                    variant="outline"
                    className="flex items-center gap-1 rounded-full px-3 py-1"
                  >
                    <Target className="h-3 w-3" /> Objectif : zéro dette
                  </Badge>
                </div>
              </div>
            </div>

            <Separator className="my-6" />

            <div className="space-y-5">
              <div>
                <h3 className="font-display text-lg font-semibold text-card-foreground mb-4">
                  Informations personnelles
                </h3>

                <div className="grid md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label className="flex items-center gap-2">
                      <User className="h-4 w-4" />
                      Nom complet
                    </Label>
                    <Input defaultValue="John Doe" className="rounded-xl h-11" />
                  </div>

                  <div className="space-y-2">
                    <Label className="flex items-center gap-2">
                      <Mail className="h-4 w-4" />
                      Adresse e-mail
                    </Label>
                    <Input
                      defaultValue="john@example.com"
                      type="email"
                      className="rounded-xl h-11"
                    />
                  </div>
                </div>
              </div>
            </div>

            <Separator className="my-6" />

            <div>
              <h3 className="font-display text-lg font-semibold text-card-foreground mb-4 flex items-center gap-2">
                <Bell className="h-4 w-4" />
                Préférences de notification
              </h3>

              <div className="space-y-4">
                <label className="flex items-start gap-3 rounded-xl bg-muted/20 px-4 py-3">
                  <input
                    type="checkbox"
                    defaultChecked
                    className="rounded border-input mt-1"
                  />
                  <div>
                    <p className="text-card-foreground font-medium text-sm">
                      Rappels de paiement
                    </p>
                    <p className="text-xs text-muted-foreground">
                      Reçois une alerte avant tes échéances importantes.
                    </p>
                  </div>
                </label>

                <label className="flex items-start gap-3 rounded-xl bg-muted/20 px-4 py-3">
                  <input
                    type="checkbox"
                    defaultChecked
                    className="rounded border-input mt-1"
                  />
                  <div>
                    <p className="text-card-foreground font-medium text-sm">
                      Mises à jour des défis
                    </p>
                    <p className="text-xs text-muted-foreground">
                      Sois informé(e) des progrès et classements de ton groupe.
                    </p>
                  </div>
                </label>

                <label className="flex items-start gap-3 rounded-xl bg-muted/20 px-4 py-3">
                  <input type="checkbox" className="rounded border-input mt-1" />
                  <div>
                    <p className="text-card-foreground font-medium text-sm">
                      Résumé hebdomadaire
                    </p>
                    <p className="text-xs text-muted-foreground">
                      Reçois un bilan de ta progression chaque semaine.
                    </p>
                  </div>
                </label>
              </div>
            </div>

            <Separator className="my-6" />

            <div>
              <h3 className="font-display text-lg font-semibold text-card-foreground mb-4 flex items-center gap-2">
                <Shield className="h-4 w-4" />
                Sécurité
              </h3>

              <div className="flex flex-wrap gap-3">
                <Button variant="outline" className="rounded-xl">
                  Modifier le mot de passe
                </Button>
                <Button variant="outline" className="rounded-xl">
                  Gérer la sécurité du compte
                </Button>
              </div>
            </div>

            <div className="mt-6 pt-2">
              <Button className="gradient-primary border-0 rounded-xl">
                Enregistrer les modifications
              </Button>
            </div>
          </Card>
        </div>

        {/* Side summary */}
        <div className="space-y-6">
          <Card className="p-5 rounded-2xl shadow-card border bg-card/90">
            <h3 className="font-display text-lg font-semibold text-card-foreground mb-4">
              Résumé du compte
            </h3>

            <div className="space-y-4">
              <div className="rounded-xl bg-muted/20 p-4">
                <p className="text-xs text-muted-foreground mb-1">
                  Stratégie active
                </p>
                <p className="font-display font-bold text-card-foreground">
                  Avalanche
                </p>
              </div>

              <div className="rounded-xl bg-muted/20 p-4">
                <p className="text-xs text-muted-foreground mb-1">
                  Budget mensuel
                </p>
                <p className="font-display font-bold text-card-foreground">
                  $600 / mois
                </p>
              </div>

              <div className="rounded-xl bg-muted/20 p-4">
                <p className="text-xs text-muted-foreground mb-1">
                  Date estimée de fin
                </p>
                <p className="font-display font-bold text-card-foreground">
                  Juin 2027
                </p>
              </div>
            </div>
          </Card>

          <Card className="p-5 rounded-2xl shadow-card border bg-card/90">
            <h3 className="font-display text-lg font-semibold text-card-foreground mb-4">
              Activité récente
            </h3>

            <div className="space-y-4 text-sm">
              <div className="rounded-xl bg-muted/20 p-4">
                <p className="font-medium text-card-foreground">
                  Paiement enregistré
                </p>
                <p className="text-muted-foreground">
                  $600 ajoutés à ton plan cette semaine.
                </p>
              </div>

              <div className="rounded-xl bg-muted/20 p-4">
                <p className="font-medium text-card-foreground">
                  Défi en cours
                </p>
                <p className="text-muted-foreground">
                  Tu es actuellement #1 dans “Debt Crushers 2025”.
                </p>
              </div>

              <div className="rounded-xl bg-muted/20 p-4">
                <p className="font-medium text-card-foreground">
                  Régularité maintenue
                </p>
                <p className="text-muted-foreground">
                  14 jours de suivi sans interruption.
                </p>
              </div>
            </div>
          </Card>

          <Card className="p-5 rounded-2xl shadow-card border bg-card/90">
            <h3 className="font-display text-lg font-semibold text-card-foreground mb-4 flex items-center gap-2">
              <CalendarDays className="h-4 w-4" />
              Statut
            </h3>

            <div className="space-y-3">
              <Badge className="gradient-success border-0 rounded-full px-3 py-1">
                Compte actif
              </Badge>
              <p className="text-sm text-muted-foreground">
                Ton profil est complet et tes préférences sont enregistrées.
              </p>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default Profile;