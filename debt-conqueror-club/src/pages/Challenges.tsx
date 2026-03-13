import { useMemo, useState } from "react";
import { mockChallenges, type Challenge } from "@/lib/mockData";
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
  const [challenges, setChallenges] = useState(mockChallenges);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: "", goal: "", endDate: "" });

  const stats = useMemo(() => {
    const activeChallenges = challenges.length;
    const totalRepaid = challenges.reduce((sum, c) => sum + c.currentAmount, 0);
    const totalMembers = challenges.reduce((sum, c) => sum + c.members.length, 0);

    const bestRank = challenges.reduce((best, challenge) => {
      const sorted = [...challenge.members].sort(
        (a, b) => b.contributed - a.contributed
      );
      const myIndex = sorted.findIndex((m) => m.name.toLowerCase() === "you");
      if (myIndex === -1) return best;
      const rank = myIndex + 1;
      return best === null ? rank : Math.min(best, rank);
    }, null as number | null);

    return {
      activeChallenges,
      totalRepaid,
      totalMembers,
      bestRank,
    };
  }, [challenges]);

  const handleCreate = () => {
    if (!form.name || !form.goal) return;

    const newChallenge: Challenge = {
      id: Date.now().toString(),
      name: form.name,
      goal: +form.goal,
      currentAmount: 0,
      startDate: new Date().toISOString().split("T")[0],
      endDate: form.endDate || "2025-12-31",
      members: [
        {
          id: "1",
          name: "You",
          avatar: "Y",
          contributed: 0,
          streak: 0,
        },
      ],
    };

    setChallenges([newChallenge, ...challenges]);
    toast.success("challenge créé ! Tu peux maintenant inviter des participants.");
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
              >
                Créer le challenge
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
      <div className="space-y-6">
        {challenges.map((challenge) => {
          const pct = Math.round((challenge.currentAmount / challenge.goal) * 100);
          const sorted = [...challenge.members].sort(
            (a, b) => b.contributed - a.contributed
          );
          const myIndex = sorted.findIndex(
            (m) => m.name.toLowerCase() === "you"
          );
          const myRank = myIndex >= 0 ? myIndex + 1 : null;
          const myContribution =
            sorted.find((m) => m.name.toLowerCase() === "you")?.contributed ?? 0;

          const remainingAmount = Math.max(
            0,
            challenge.goal - challenge.currentAmount
          );
          const daysRemaining = getDaysRemaining(challenge.endDate);
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
                      {challenge.name}
                    </h3>
                    <Badge variant="outline" className="rounded-full">
                      {status}
                    </Badge>
                  </div>

                  <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
                    <span className="flex items-center gap-1">
                      <Users className="h-3.5 w-3.5" />
                      {challenge.members.length} membres
                    </span>
                    <span className="flex items-center gap-1">
                      <Target className="h-3.5 w-3.5" />
                      Objectif ${challenge.goal.toLocaleString()}
                    </span>
                    <span className="flex items-center gap-1">
                      <CalendarDays className="h-3.5 w-3.5" />
                      {challenge.startDate} → {challenge.endDate}
                    </span>
                  </div>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 w-full lg:w-auto">
                  <div className="rounded-xl bg-muted/40 px-4 py-3 min-w-[145px]">
                    <p className="text-xs text-muted-foreground mb-1">
                      Reste à atteindre
                    </p>
                    <p className="font-display font-bold text-card-foreground">
                      ${remainingAmount.toLocaleString()}
                    </p>
                  </div>

                  <div className="rounded-xl bg-muted/40 px-4 py-3 min-w-[145px]">
                    <p className="text-xs text-muted-foreground mb-1">
                      Temps restant
                    </p>
                    <p className="font-display font-bold text-card-foreground">
                      {daysRemaining >= 0 ? `${daysRemaining} jours` : "Terminé"}
                    </p>
                  </div>

                  <div className="rounded-xl bg-primary/5 px-4 py-3 min-w-[145px]">
                    <p className="text-xs text-muted-foreground mb-1">Ton rang</p>
                    <p className="font-display font-bold text-primary">
                      {myRank ? `#${myRank}` : "—"}
                    </p>
                  </div>
                </div>
              </div>

              {/* Progress */}
              <div className="space-y-2 mb-6">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">
                    ${challenge.currentAmount.toLocaleString()} sur $
                    {challenge.goal.toLocaleString()}
                  </span>
                  <span className="font-semibold text-card-foreground">
                    {pct}%
                  </span>
                </div>
                <Progress value={pct} className="h-3" />
              </div>

              {/* Personal summary */}
              <div className="grid md:grid-cols-3 gap-4 mb-6">
                <div className="rounded-xl bg-muted/30 p-4">
                  <p className="text-xs text-muted-foreground mb-1">
                    Ta contribution
                  </p>
                  <p className="text-lg font-display font-bold text-card-foreground">
                    ${myContribution.toLocaleString()}
                  </p>
                </div>

                <div className="rounded-xl bg-muted/30 p-4">
                  <p className="text-xs text-muted-foreground mb-1">
                    Progression collective
                  </p>
                  <p className="text-lg font-display font-bold text-card-foreground">
                    {pct}% atteint
                  </p>
                </div>

                <div className="rounded-xl bg-muted/30 p-4">
                  <p className="text-xs text-muted-foreground mb-1">
                    Dynamique du groupe
                  </p>
                  <p className="text-lg font-display font-bold text-card-foreground">
                    {challenge.members.length} participants
                  </p>
                </div>
              </div>

              {/* Leaderboard */}
              <h4 className="font-display font-semibold text-sm text-card-foreground mb-3 flex items-center gap-2">
                <Medal className="h-4 w-4 text-primary" />
                Leaderboard
              </h4>

              <div className="space-y-3">
                {sorted.map((member, i) => (
                  <div
                    key={member.id}
                    className="flex items-center gap-3 rounded-xl bg-muted/20 px-3 py-3"
                  >
                    <span
                      className={`w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold ${
                        i === 0
                          ? "gradient-primary text-primary-foreground"
                          : "bg-muted text-muted-foreground"
                      }`}
                    >
                      {i + 1}
                    </span>

                    <div className="w-9 h-9 rounded-full bg-primary/10 flex items-center justify-center text-sm font-semibold text-primary">
                      {member.avatar}
                    </div>

                    <div className="flex flex-col">
                      <span className="font-medium text-card-foreground text-sm">
                        {member.name}
                      </span>
                      {member.name.toLowerCase() === "you" && (
                        <span className="text-xs text-primary">Toi</span>
                      )}
                    </div>

                    <span className="flex items-center gap-1 text-xs text-warning ml-2">
                      <Flame className="h-3 w-3" />
                      {member.streak}j
                    </span>

                    <span className="ml-auto font-semibold text-sm text-card-foreground">
                      ${member.contributed.toLocaleString()}
                    </span>
                  </div>
                ))}
              </div>

              {/* Actions */}
              <div className="flex flex-wrap gap-3 mt-5">
                <Button className="gradient-primary border-0 rounded-xl">
                  Voir le challenge
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>

                <Button variant="outline" className="rounded-xl">
                  <Users className="h-4 w-4 mr-2" />
                  Inviter
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