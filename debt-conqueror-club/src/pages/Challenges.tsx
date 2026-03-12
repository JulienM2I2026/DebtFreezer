import { useMemo, useState, useEffect } from "react";
import { getChallenges, createChallenge, type ChallengeDto } from "@/apis/ChallengeApi";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import {
  Plus,
  Trophy,
  Users,
  Flame,
  Target,
  Medal,
  CalendarDays,
  ArrowRight,
  Wallet,
  Crown,
  MessageCircle,
} from "lucide-react";
import { toast } from "sonner";

const Challenges = () => {
  const [challenges, setChallenges] = useState<ChallengeDto[]>([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: "", goal: "", endDate: "" });
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    getChallenges().then(setChallenges);
  }, []);

  const stats = useMemo(() => {
    const activeChallenges = challenges.filter((c) => c.status === 0).length;
    const totalRepaid = challenges.reduce((sum, c) => sum + c.totalPaid, 0);
    const totalMembers = challenges.reduce((sum, c) => sum + c.participantCount, 0);
    return { activeChallenges, totalRepaid, totalMembers, bestRank: null as number | null };
  }, [challenges]);

  const handleCreate = async () => {
    if (!form.name || !form.goal) return;
    setCreating(true);
    const result = await createChallenge({
      title: form.name,
      targetAmount: +form.goal,
      dueDate: form.endDate || new Date(Date.now() + 90 * 24 * 60 * 60 * 1000).toISOString(),
    });
    if (result) {
      setChallenges((prev) => [result, ...prev]);
      toast.success("Challenge créé ! Tu peux maintenant inviter des participants.");
    } else {
      toast.error("Erreur lors de la création du challenge.");
    }
    setCreating(false);
    setOpen(false);
    setForm({ name: "", goal: "", endDate: "" });
  };

  const getDaysRemaining = (endDate: string) => {
    const today = new Date();
    const end = new Date(endDate);
    const diff = end.getTime() - today.getTime();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  };

  const getStatus = (pct: number, daysRemaining: number) => {
    if (pct >= 100) return "Objectif atteint";
    if (daysRemaining < 0) return "Terminé";
    if (daysRemaining <= 14) return "Bientôt terminé";
    return "En cours";
  };

  return (
    <div className="space-y-6 max-w-6xl mx-auto animate-fade-in pb-8">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-display text-3xl font-bold text-foreground">
            Challenges
          </h1>
          <p className="text-muted-foreground">
            Atteins tes objectifs de remboursement avec tes proches.
          </p>
        </div>

        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button className="gradient-primary border-0 rounded-xl">
              <Plus className="h-4 w-4 mr-2" />
              Créer un challenge
            </Button>
          </DialogTrigger>

          <DialogContent>
            <DialogHeader>
              <DialogTitle className="font-display">
                Créer un nouveau challenge
              </DialogTitle>
            </DialogHeader>

            <div className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label>Nom du challenge</Label>
                <Input
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  placeholder="Ex. Debt Crushers 2025"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Objectif collectif ($)</Label>
                  <Input
                    type="number"
                    value={form.goal}
                    onChange={(e) => setForm({ ...form, goal: e.target.value })}
                    placeholder="10000"
                  />
                </div>

                <div className="space-y-2">
                  <Label>Date de fin</Label>
                  <Input
                    type="date"
                    value={form.endDate}
                    onChange={(e) =>
                      setForm({ ...form, endDate: e.target.value })
                    }
                  />
                </div>
              </div>

              <Button
                className="w-full gradient-primary border-0 rounded-xl"
                onClick={handleCreate}
                disabled={creating}
              >
                {creating ? "Création…" : "Créer le challenge"}
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      {/* KPI row */}
      <div className="grid md:grid-cols-4 gap-4">
        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center mb-3">
            <Trophy className="h-5 w-5 text-primary" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Challenges actifs</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            {stats.activeChallenges}
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-success/10 flex items-center justify-center mb-3">
            <Wallet className="h-5 w-5 text-success" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">
            Remboursé ensemble
          </p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            ${stats.totalRepaid.toLocaleString()}
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-info/10 flex items-center justify-center mb-3">
            <Users className="h-5 w-5 text-info" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Participants</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            {stats.totalMembers}
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="w-10 h-10 rounded-xl bg-warning/10 flex items-center justify-center mb-3">
            <Crown className="h-5 w-5 text-warning" />
          </div>
          <p className="text-sm text-muted-foreground mb-1">Meilleur rang</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            {stats.bestRank ? `#${stats.bestRank}` : "—"}
          </p>
        </Card>
      </div>

      {/* Challenge cards */}
      {challenges.length === 0 && (
        <Card className="p-10 rounded-2xl border bg-card/80 text-center">
          <p className="text-muted-foreground">
            Aucun challenge pour le moment. Crée le premier !
          </p>
        </Card>
      )}

      <div className="space-y-6">
        {challenges.map((challenge) => {
          const pct = Math.round(challenge.progressPercent ?? 0);
          const remainingAmount = Math.max(0, challenge.targetAmount - challenge.totalPaid);
          const daysRemaining = getDaysRemaining(challenge.dueDate);
          const status = getStatus(pct, daysRemaining);

          return (
            <Card
              key={challenge.id}
              className="p-6 shadow-card rounded-2xl border bg-card/90"
            >
              {/* Header */}
              <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4 mb-5">
                <div>
                  <div className="flex items-center gap-2 mb-2">
                    <Trophy className="h-5 w-5 text-warning" />
                    <h3 className="font-display text-xl font-semibold text-card-foreground">
                      {challenge.title}
                    </h3>
                    <Badge variant="outline" className="rounded-full">
                      {status}
                    </Badge>
                  </div>

                  <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
                    <span className="flex items-center gap-1">
                      <Users className="h-3.5 w-3.5" />
                      {challenge.participantCount} participant(s)
                    </span>
                    <span className="flex items-center gap-1">
                      <Target className="h-3.5 w-3.5" />
                      Objectif ${challenge.targetAmount.toLocaleString()}
                    </span>
                    <span className="flex items-center gap-1">
                      <CalendarDays className="h-3.5 w-3.5" />
                      Fin : {new Date(challenge.dueDate).toLocaleDateString("fr-FR")}
                    </span>
                  </div>
                  {challenge.description && (
                    <p className="text-sm text-muted-foreground mt-2">{challenge.description}</p>
                  )}
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 w-full lg:w-auto">
                  <div className="rounded-xl bg-muted/40 px-4 py-3 min-w-[145px]">
                    <p className="text-xs text-muted-foreground mb-1">Reste à atteindre</p>
                    <p className="font-display font-bold text-card-foreground">
                      ${remainingAmount.toLocaleString()}
                    </p>
                  </div>

                  <div className="rounded-xl bg-muted/40 px-4 py-3 min-w-[145px]">
                    <p className="text-xs text-muted-foreground mb-1">Temps restant</p>
                    <p className="font-display font-bold text-card-foreground">
                      {daysRemaining >= 0 ? `${daysRemaining} jours` : "Terminé"}
                    </p>
                  </div>
                </div>
              </div>

              {/* Progress */}
              <div className="space-y-2 mb-6">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">
                    ${challenge.totalPaid.toLocaleString()} sur ${challenge.targetAmount.toLocaleString()}
                  </span>
                  <span className="font-semibold text-card-foreground">{pct}%</span>
                </div>
                <Progress value={pct} className="h-3" />
              </div>

              {/* Summary */}
              <div className="grid md:grid-cols-2 gap-4 mb-6">
                <div className="rounded-xl bg-muted/30 p-4">
                  <p className="text-xs text-muted-foreground mb-1">Montant remboursé</p>
                  <p className="text-lg font-display font-bold text-card-foreground">
                    ${challenge.totalPaid.toLocaleString()}
                  </p>
                </div>
                <div className="rounded-xl bg-muted/30 p-4">
                  <p className="text-xs text-muted-foreground mb-1">Participants</p>
                  <p className="text-lg font-display font-bold text-card-foreground">
                    {challenge.participantCount}
                  </p>
                </div>
              </div>

              {/* Actions */}
              <div className="flex flex-wrap gap-3 mt-5">
                <Button className="gradient-primary border-0 rounded-xl">
                  Voir le challenge
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>
                <Button variant="outline" className="rounded-xl">
                  <Users className="h-4 w-4 mr-2" />
                  Rejoindre
                </Button>
                <Button variant="outline" className="rounded-xl">
                  <Wallet className="h-4 w-4 mr-2" />
                  Ajouter un paiement
                </Button>
                <Button variant="ghost" className="rounded-xl">
                  <MessageCircle className="h-4 w-4 mr-2" />
                  Voir le chat
                </Button>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
};

export default Challenges;