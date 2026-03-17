import { useState, useMemo, useEffect } from "react";
import { getDebts } from "@/apis/DebtApi";
import { calculateStrategy } from "@/apis/StrategyApi";
import { toast } from "sonner";
import { Card } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  TrendingDown,
  Snowflake,
  ArrowDown,
  DollarSign,
  Clock,
  Percent,
  Sparkles,
  Wallet,
  Gauge,
  Target,
} from "lucide-react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

type DebtForSimulation = {
  id: number | string;
  creditor: string;
  remainingAmount: number;
  interestRate: number;
};

function simulatePayoff(
  debts: DebtForSimulation[],
  strategy: "avalanche" | "snowball",
  monthlyBudget: number
) {
  let remaining = debts.map((d) => ({ ...d, balance: d.remainingAmount }));
  let months = 0;
  let totalInterest = 0;
  const timeline: { month: number; total: number }[] = [];

  while (remaining.some((d) => d.balance > 0) && months < 360) {
    months++;
    let budget = monthlyBudget;

    // Appliquer les intérêts mensuels
    for (const d of remaining) {
      if (d.balance > 0) {
        const interest = (d.balance * d.interestRate) / 100 / 12;
        d.balance += interest;
        totalInterest += interest;
      }
    }

    // Trier selon la stratégie
    const sorted = [...remaining].sort((a, b) =>
      strategy === "avalanche"
        ? b.interestRate - a.interestRate
        : a.balance - b.balance
    );

    // Appliquer le budget mensuel dans l'ordre de priorité
    for (const s of sorted) {
      const d = remaining.find((r) => r.id === s.id)!;
      if (d.balance > 0 && budget > 0) {
        const payment = Math.min(budget, d.balance);
        d.balance -= payment;
        budget -= payment;
      }
    }

    if (months % 3 === 0 || !remaining.some((d) => d.balance > 0)) {
      timeline.push({
        month: months,
        total: Math.round(remaining.reduce((sum, d) => sum + Math.max(0, d.balance), 0)),
      });
    }
  }

  const order =
    strategy === "avalanche"
      ? [...debts].sort((a, b) => b.interestRate - a.interestRate)
      : [...debts].sort((a, b) => a.remainingAmount - b.remainingAmount);

  return { months, totalInterest: Math.round(totalInterest), timeline, order };
}

const Strategy = () => {
  const [extraPayment, setExtraPayment] = useState(0);
  const [debts, setDebts] = useState<DebtForSimulation[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const monthlyRepaymentBudget = Number(localStorage.getItem("monthlyRepaymentBudget") ?? 0);

  useEffect(() => {
    getDebts().then((data) => {
      const active = (data ?? []).filter((d: any) => d.status === 0);
      setDebts(active.map((d: any) => ({
        id: d.id,
        creditor: d.creditor,
        remainingAmount: d.remainingAmount,
        interestRate: d.interestRate,
      })));
      setExtraPayment(monthlyRepaymentBudget || 200);
      setLoading(false);
    });
  }, []);

  const avalanche = useMemo(
    () => simulatePayoff(debts, "avalanche", extraPayment),
    [debts, extraPayment]
  );

  const snowball = useMemo(
    () => simulatePayoff(debts, "snowball", extraPayment),
    [debts, extraPayment]
  );

  const savings = snowball.totalInterest - avalanche.totalInterest;
  const totalDebt = debts.reduce((sum, d) => sum + d.remainingAmount, 0);

  const handleChoose = async (strategyType: 1 | 2) => {
    setSaving(true);
    const result = await calculateStrategy({ monthlyBudget: extraPayment, strategyType });
    if (result) {
      localStorage.setItem("strategy", String(strategyType));
      toast.success(`Stratégie ${strategyType === 1 ? "Snowball" : "Avalanche"} enregistrée !`);
    } else {
      toast.error("Erreur lors de l'enregistrement de la stratégie.");
    }
    setSaving(false);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <p className="text-muted-foreground">Chargement des dettes…</p>
      </div>
    );
  }

  if (debts.length === 0) {
    return (
      <div className="space-y-4 max-w-6xl mx-auto animate-fade-in pb-8">
        <h1 className="font-display text-3xl font-bold text-foreground">Simulateur de remboursement</h1>
        <p className="text-muted-foreground">Aucune dette active. Ajoutez des dettes pour utiliser le simulateur.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-6xl mx-auto animate-fade-in pb-8">
      <div className="space-y-1">
        <h1 className="font-display text-3xl font-bold text-foreground">
          Simulateur de remboursement
        </h1>
        <p className="text-muted-foreground">
          Compare Avalanche et Snowball pour choisir la stratégie la plus adaptée.
        </p>
      </div>

      <Card className="p-5 rounded-2xl border bg-primary/5 border-primary/10 shadow-card">
        <div className="flex items-start gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
            <Sparkles className="h-5 w-5 text-primary" />
          </div>

          <div className="space-y-1">
            <h2 className="font-display text-lg font-semibold text-foreground">
              Synthèse de la simulation
            </h2>
            <p className="text-sm text-muted-foreground">
              {savings > 0
                ? `La méthode Avalanche permet d’économiser environ $${savings.toLocaleString()} d’intérêts par rapport à Snowball.`
                : "Les deux méthodes présentent ici un coût très proche."}
            </p>
            <p className="text-sm text-muted-foreground">
              La méthode Snowball peut toutefois être plus motivante grâce à des
              dettes soldées plus rapidement.
            </p>
          </div>
        </div>
      </Card>

      <div className="grid md:grid-cols-4 gap-4">
        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="flex items-start justify-between mb-3">
            <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center">
              <Wallet className="h-5 w-5 text-primary" />
            </div>
          </div>
          <p className="text-sm text-muted-foreground mb-1">Dette totale</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            {totalDebt.toLocaleString("fr-FR", { style: "currency", currency: "EUR" })}
          </p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="flex items-start justify-between mb-3">
            <div className="w-10 h-10 rounded-xl bg-info/10 flex items-center justify-center">
              <Gauge className="h-5 w-5 text-info" />
            </div>
          </div>
          <p className="text-sm text-muted-foreground mb-1">Budget de remboursement</p>
          <p className="text-2xl font-display font-bold text-card-foreground">
            {(monthlyRepaymentBudget ?? 0).toLocaleString("fr-FR", { style: "currency", currency: "EUR" })}
          </p>
          <p className="text-xs text-muted-foreground mt-1">Budget mensuel enregistré</p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="flex items-start justify-between mb-3">
            <div className="w-10 h-10 rounded-xl bg-success/10 flex items-center justify-center">
              <Target className="h-5 w-5 text-success" />
            </div>
          </div>
          <p className="text-sm text-muted-foreground mb-1">Budget total simulé</p>
          <p className="text-2xl font-display font-bold text-success">
            {((monthlyRepaymentBudget ?? 0) + extraPayment).toLocaleString("fr-FR", { style: "currency", currency: "EUR" })}
          </p>
          <p className="text-xs text-muted-foreground mt-1">Budget initial + supplément</p>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card border bg-card/80">
          <div className="flex items-start justify-between mb-3">
            <div className="w-10 h-10 rounded-xl bg-warning/10 flex items-center justify-center">
              <Percent className="h-5 w-5 text-warning" />
            </div>
          </div>
          <p className="text-sm text-muted-foreground mb-1">Économie potentielle</p>
          <p className="text-2xl font-display font-bold text-warning">
            {savings > 0 ? savings.toLocaleString("fr-FR", { style: "currency", currency: "EUR" }) : "—"}
          </p>
        </Card>
      </div>

      <Card className="p-6 shadow-card rounded-2xl border bg-card/80">
        <div className="flex flex-col md:flex-row md:items-end md:justify-between gap-4">
          <div className="space-y-2">
            <Label className="text-sm font-semibold">
              Budget mensuel supplémentaire
            </Label>
            <p className="text-sm text-muted-foreground">
              S'ajoute au budget de remboursement enregistré
              {monthlyRepaymentBudget > 0 ? ` (${monthlyRepaymentBudget.toLocaleString("fr-FR", { style: "currency", currency: "EUR" })})` : ""}.
            </p>
          </div>

          <div className="flex items-center gap-3">
            <Input
              type="number"
              value={extraPayment}
              onChange={(e) => setExtraPayment(+e.target.value)}
              className="w-40 h-12 rounded-xl text-base"
              placeholder="200"
            />
            <Badge variant="secondary" className="rounded-full px-3 py-1">
              en plus des minimums
            </Badge>
          </div>
        </div>
      </Card>

      <div className="grid lg:grid-cols-2 gap-5 items-stretch">
        <Card className="p-6 shadow-card rounded-2xl border-l-4 border-l-primary h-full flex flex-col">
          <div className="flex items-start gap-3 mb-5">
            <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center">
              <TrendingDown className="h-5 w-5 text-primary" />
            </div>

            <div className="flex-1">
              <div className="flex items-center gap-2">
                <h3 className="font-display text-xl font-semibold text-card-foreground">
                  Méthode Avalanche
                </h3>
                {savings > 0 && (
                  <Badge className="gradient-primary border-0 text-xs">
                    Recommandée
                  </Badge>
                )}
              </div>
              <p className="text-sm text-muted-foreground mt-1">
                Priorise les taux d’intérêt les plus élevés pour réduire le coût total.
              </p>
            </div>
          </div>

          <div className="grid grid-cols-3 gap-4 mb-5">
            <div className="rounded-xl bg-muted/40 p-4 min-h-[96px] flex flex-col justify-between">
              <p className="text-xs text-muted-foreground flex items-center gap-1 mb-1">
                <Clock className="h-3 w-3" /> Durée estimée
              </p>
              <p className="text-xl font-display font-bold text-card-foreground">
                {Math.round(avalanche.months / 12)}a {avalanche.months % 12}m
              </p>
            </div>

            <div className="rounded-xl bg-muted/40 p-4 min-h-[96px] flex flex-col justify-between">
              <p className="text-xs text-muted-foreground flex items-center gap-1 mb-1">
                <DollarSign className="h-3 w-3" /> Intérêts totaux
              </p>
              <p className="text-xl font-display font-bold text-card-foreground">
                ${avalanche.totalInterest.toLocaleString()}
              </p>
            </div>

            <div className="rounded-xl bg-success/5 p-4 min-h-[96px] flex flex-col justify-between">
              <p className="text-xs text-muted-foreground flex items-center gap-1 mb-1">
                <Percent className="h-3 w-3" /> Économie
              </p>
              <p className="text-xl font-display font-bold text-success">
                {savings > 0 ? `$${savings.toLocaleString()}` : "—"}
              </p>
            </div>
          </div>

          <div className="space-y-3">
            <p className="text-sm font-semibold text-card-foreground">
              Ordre de remboursement
            </p>

            {avalanche.order.map((d, i) => (
              <div
                key={d.id}
                className="flex items-center gap-3 rounded-xl bg-muted/30 px-3 py-3 text-sm"
              >
                <span className="w-6 h-6 rounded-full bg-primary/10 text-primary text-xs flex items-center justify-center font-bold">
                  {i + 1}
                </span>
                <span className="text-card-foreground">{d.creditor}</span>
                <span className="ml-auto text-muted-foreground text-xs">
                  {d.interestRate}%
                </span>
              </div>
            ))}
          </div>

          <Button
            className="w-full mt-5 rounded-xl gradient-primary border-0 mt-auto"
            onClick={() => handleChoose(2)}
            disabled={saving}
          >
            {saving ? "Enregistrement…" : "Choisir Avalanche"}
          </Button>
        </Card>

        <Card className="p-6 shadow-card rounded-2xl border-l-4 border-l-info h-full flex flex-col">
          <div className="flex items-start gap-3 mb-5">
            <div className="w-10 h-10 rounded-xl bg-info/10 flex items-center justify-center">
              <Snowflake className="h-5 w-5 text-info" />
            </div>

            <div className="flex-1">
              <h3 className="font-display text-xl font-semibold text-card-foreground">
                Méthode Snowball
              </h3>
              <p className="text-sm text-muted-foreground mt-1">
                Priorise les plus petits soldes pour obtenir des victoires rapides.
              </p>
            </div>
          </div>

          <div className="grid grid-cols-3 gap-4 mb-5">
            <div className="rounded-xl bg-muted/40 p-4 min-h-[96px] flex flex-col justify-between">
              <p className="text-xs text-muted-foreground flex items-center gap-1 mb-1">
                <Clock className="h-3 w-3" /> Durée estimée
              </p>
              <p className="text-xl font-display font-bold text-card-foreground">
                {Math.round(snowball.months / 12)}a {snowball.months % 12}m
              </p>
            </div>

            <div className="rounded-xl bg-muted/40 p-4 min-h-[96px] flex flex-col justify-between">
              <p className="text-xs text-muted-foreground flex items-center gap-1 mb-1">
                <DollarSign className="h-3 w-3" /> Intérêts totaux
              </p>
              <p className="text-xl font-display font-bold text-card-foreground">
                ${snowball.totalInterest.toLocaleString()}
              </p>
            </div>

            <div className="rounded-xl bg-info/5 p-4 min-h-[96px] flex flex-col justify-between">
              <p className="text-xs text-muted-foreground flex items-center gap-1 mb-1">
                <ArrowDown className="h-3 w-3" /> Motivation
              </p>
              <p className="text-lg font-display font-bold text-info leading-tight">
                Victoires rapides
              </p>
            </div>
          </div>

          <div className="space-y-3">
            <p className="text-sm font-semibold text-card-foreground">
              Ordre de remboursement
            </p>

            {snowball.order.map((d, i) => (
              <div
                key={d.id}
                className="flex items-center gap-3 rounded-xl bg-muted/30 px-3 py-3 text-sm"
              >
                <span className="w-6 h-6 rounded-full bg-info/10 text-info text-xs flex items-center justify-center font-bold">
                  {i + 1}
                </span>
                <span className="text-card-foreground">{d.creditor}</span>
                <span className="ml-auto text-muted-foreground text-xs">
                  ${d.remainingAmount.toLocaleString()}
                </span>
              </div>
            ))}
          </div>

          <Button
            variant="outline"
            className="w-full mt-5 rounded-xl mt-auto"
            onClick={() => handleChoose(1)}
            disabled={saving}
          >
            {saving ? "Enregistrement…" : "Choisir Snowball"}
          </Button>
        </Card>
      </div>

      <Card className="p-6 shadow-card rounded-2xl">
        <div className="flex items-center gap-2 mb-1">
          <Sparkles className="h-4 w-4 text-primary" />
          <h3 className="font-display text-xl font-semibold text-card-foreground">
            Projection du solde restant
          </h3>
        </div>
        <p className="text-sm text-muted-foreground mb-5">
          Visualise l’évolution de ta dette dans le temps selon la stratégie choisie.
        </p>

        <Tabs defaultValue="avalanche">
          <TabsList className="mb-6 rounded-xl">
            <TabsTrigger value="avalanche">Avalanche</TabsTrigger>
            <TabsTrigger value="snowball">Snowball</TabsTrigger>
          </TabsList>

          <TabsContent value="avalanche">
            <ResponsiveContainer width="100%" height={320}>
              <BarChart data={avalanche.timeline}>
                <CartesianGrid strokeDasharray="3 3" stroke="hsl(220, 13%, 89%)" />
                <XAxis
                  dataKey="month"
                  tick={{ fontSize: 12 }}
                  stroke="hsl(220, 10%, 46%)"
                  label={{ value: "Mois", position: "insideBottom", offset: -5 }}
                />
                <YAxis
                  tick={{ fontSize: 12 }}
                  stroke="hsl(220, 10%, 46%)"
                  tickFormatter={(v) => `$${(v / 1000).toFixed(0)}k`}
                />
                <Tooltip formatter={(v: number) => `$${v.toLocaleString()}`} />
                <Bar
                  dataKey="total"
                  fill="hsl(239, 84%, 67%)"
                  radius={[6, 6, 0, 0]}
                />
              </BarChart>
            </ResponsiveContainer>
          </TabsContent>

          <TabsContent value="snowball">
            <ResponsiveContainer width="100%" height={320}>
              <BarChart data={snowball.timeline}>
                <CartesianGrid strokeDasharray="3 3" stroke="hsl(220, 13%, 89%)" />
                <XAxis
                  dataKey="month"
                  tick={{ fontSize: 12 }}
                  stroke="hsl(220, 10%, 46%)"
                  label={{ value: "Mois", position: "insideBottom", offset: -5 }}
                />
                <YAxis
                  tick={{ fontSize: 12 }}
                  stroke="hsl(220, 10%, 46%)"
                  tickFormatter={(v) => `$${(v / 1000).toFixed(0)}k`}
                />
                <Tooltip formatter={(v: number) => `$${v.toLocaleString()}`} />
                <Bar
                  dataKey="total"
                  fill="hsl(217, 91%, 60%)"
                  radius={[6, 6, 0, 0]}
                />
              </BarChart>
            </ResponsiveContainer>
          </TabsContent>
        </Tabs>
      </Card>
    </div>
  );
};

export default Strategy;