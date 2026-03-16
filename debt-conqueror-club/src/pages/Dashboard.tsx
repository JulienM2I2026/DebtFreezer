import { useState, useEffect, useMemo } from "react";
import { getDebts, getDebtByMonth } from "@/apis/DebtApi";
import { getPayments } from "@/apis/PaymentApi";
import { Card } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { Badge } from "@/components/ui/badge";
import {
  CreditCard,
  Percent,
  DollarSign,
  Trophy,
  Flame,
  Target,
  Wallet,
  Sparkles,
  ArrowRight,
} from "lucide-react";

const DEBT_TYPE_COLORS = ["hsl(239, 84%, 67%)", "hsl(142, 71%, 45%)", "hsl(217, 91%, 60%)"];
const DEBT_TYPE_NAMES: Record<number, string> = {
  0: "Carte crédit", 1: "Crédit personnel", 2: "Crédit étudiant"
};
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  BarChart,
  Bar,
} from "recharts";

const StatCard = ({
  icon: Icon,
  label,
  value,
  sub,
  gradient,
}: {
  icon: any;
  label: string;
  value: string;
  sub?: string;
  gradient?: boolean;
}) => (
  <Card
    className={`p-5 rounded-2xl shadow-card border ${
      gradient ? "gradient-primary text-primary-foreground border-0" : "bg-card/90"
    }`}
  >
    <div className="flex items-start justify-between gap-4">
      <div>
        <p
          className={`text-sm ${
            gradient ? "text-primary-foreground/75" : "text-muted-foreground"
          }`}
        >
          {label}
        </p>
        <p className="text-2xl font-display font-bold mt-1">{value}</p>
        {sub && (
          <p
            className={`text-xs mt-1 ${
              gradient ? "text-primary-foreground/70" : "text-muted-foreground"
            }`}
          >
            {sub}
          </p>
        )}
      </div>

      <div
        className={`w-11 h-11 rounded-xl flex items-center justify-center shrink-0 ${
          gradient ? "bg-primary-foreground/20" : "bg-primary/10"
        }`}
      >
        <Icon
          className={`h-5 w-5 ${
            gradient ? "text-primary-foreground" : "text-primary"
          }`}
        />
      </div>
    </div>
  </Card>
);

const Dashboard = () => {
  const [debts, setDebts] = useState<any[]>([]);
  const [payments, setPayments] = useState<any[]>([]);
  const [byMonth, setByMonth] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadData() {
      const [debtsData, paymentsData, byMonthData] = await Promise.all([
        getDebts(),
        getPayments(),
        getDebtByMonth(),
      ]);
      setDebts(debtsData ?? []);
      setPayments(paymentsData ?? []);
      setByMonth(byMonthData ?? []);
      setLoading(false);
    }
    loadData();
  }, []);

  const totalDebt = useMemo(() => debts.reduce((sum, d) => sum + d.remainingAmount, 0), [debts]);
  const totalOriginal = useMemo(() => debts.reduce((sum, d) => sum + d.originalAmount, 0), [debts]);
  const activeDebts = useMemo(() => debts.filter((d) => d.status === 0), [debts]);

  const avgRate = useMemo(() => {
    if (totalDebt === 0) return "0.0";
    return (debts.reduce((sum, d) => sum + d.interestRate * d.remainingAmount, 0) / totalDebt).toFixed(1);
  }, [debts, totalDebt]);

  const paidPercent = totalOriginal > 0
    ? Math.round(((totalOriginal - totalDebt) / totalOriginal) * 100)
    : 0;

  const highestRateDebt = useMemo(
    () => [...debts].sort((a, b) => b.interestRate - a.interestRate)[0],
    [debts]
  );

  // Évolution des dettes par mois pour le line chart
  const debtEvolutionData = useMemo(() => {
    const MONTH_NAMES = ["Jan", "Fév", "Mar", "Avr", "Mai", "Juin", "Juil", "Août", "Sep", "Oct", "Nov", "Déc"];
    return byMonth
      .slice(-6)
      .map((d: any) => ({
        month: `${MONTH_NAMES[d.month - 1]} ${String(d.year).slice(-2)}`,
        total: Math.round(d.totalOriginalAmount),
      }));
  }, [byMonth]);

  // Paiements groupés par mois pour le bar chart
  const paymentHistoryData = useMemo(() => {
    const grouped: Record<string, number> = {};
    payments.forEach((p) => {
      const month = new Date(p.paymentDate).toLocaleDateString("fr-FR", { month: "short", year: "2-digit" });
      grouped[month] = (grouped[month] ?? 0) + p.amount;
    });
    return Object.entries(grouped).map(([month, paid]) => ({ month, paid: Math.round(paid as number) })).slice(-6);
  }, [payments]);

  // Répartition par type de dette pour le pie chart
  const debtByTypeData = useMemo(() => {
    const types = [...new Set(debts.map((d) => d.type))] as number[];
    return types
      .map((type, i) => ({
        name: DEBT_TYPE_NAMES[type] ?? `Type ${type}`,
        value: Math.round(debts.filter((d) => d.type === type).reduce((s, d) => s + d.remainingAmount, 0)),
        fill: DEBT_TYPE_COLORS[i % DEBT_TYPE_COLORS.length],
      }))
      .filter((d) => d.value > 0);
  }, [debts]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <p className="text-muted-foreground">Chargement des données…</p>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-7xl mx-auto animate-fade-in pb-8">
      <div className="space-y-1">
        <h1 className="font-display text-3xl font-bold text-foreground">
          Tableau de bord
        </h1>
        <p className="text-muted-foreground">
          Visualise ta situation financière et pilote ton plan de remboursement.
        </p>
      </div>

      {/* Global summary */}
      <Card className="p-5 rounded-2xl border bg-primary/5 border-primary/10 shadow-card">
        <div className="flex items-start gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
            <Sparkles className="h-5 w-5 text-primary" />
          </div>

          <div className="space-y-1">
            <h2 className="font-display text-lg font-semibold text-foreground">
              Synthèse de ta situation
            </h2>
            <p className="text-sm text-muted-foreground">
              {debts.length === 0
                ? "Aucune dette enregistrée. Commence par ajouter tes dettes."
                : `Tu as remboursé ${paidPercent}% de ta dette totale.`}
            </p>
            {highestRateDebt && (
              <p className="text-sm text-muted-foreground">
                La dette la plus coûteuse est{" "}
                <strong>{highestRateDebt.creditor}</strong> avec un taux de{" "}
                <strong>{highestRateDebt.interestRate}%</strong>.
              </p>
            )}
          </div>
        </div>
      </Card>

      {/* KPI cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        <StatCard
          icon={Wallet}
          label="Dette restante"
          value={`$${totalDebt.toLocaleString()}`}
          sub={`${paidPercent}% déjà remboursé`}
          gradient
        />
        <StatCard
          icon={CreditCard}
          label="Dettes actives"
          value={String(activeDebts.length)}
          sub="Réparties sur plusieurs types"
        />
        <StatCard
          icon={Percent}
          label="Taux moyen pondéré"
          value={`${avgRate}%`}
          sub="Calculé sur les soldes restants"
        />
        <StatCard
          icon={DollarSign}
          label="Paiements enregistrés"
          value={String(payments.length)}
          sub={`Total : $${payments.reduce((s, p) => s + p.amount, 0).toLocaleString()}`}
        />
      </div>

      {/* Motivation banner */}
      <Card className="gradient-success p-4 md:p-5 flex flex-col sm:flex-row sm:items-center gap-4 border-0 rounded-2xl">
        <div className="w-10 h-10 rounded-full bg-success-foreground/20 flex items-center justify-center shrink-0">
          <Flame className="h-5 w-5 text-success-foreground" />
        </div>
        <div className="flex-1">
          <p className="text-success-foreground font-semibold text-sm">
            {payments.length > 0
              ? `${payments.length} paiement(s) enregistré(s). Continue sur ta lancée !`
              : "Enregistre ton premier paiement pour démarrer ton suivi."}
          </p>
          <p className="text-success-foreground/75 text-xs mt-0.5">
            Chaque paiement te rapproche de la liberté financière.
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Trophy className="h-4 w-4 text-success-foreground/80" />
          <span className="text-xs text-success-foreground/80">Objectif : zéro dette</span>
        </div>
      </Card>

      {/* Mid section */}
      <div className="grid xl:grid-cols-3 gap-6">
        <Card className="xl:col-span-2 p-5 rounded-2xl shadow-card bg-card/90">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h3 className="font-display font-semibold text-card-foreground">
                Évolution de la dette
              </h3>
              <p className="text-sm text-muted-foreground">
                Visualise la baisse progressive de ton solde total.
              </p>
            </div>
            <Badge variant="outline" className="rounded-full">
              6 derniers mois
            </Badge>
          </div>

          <ResponsiveContainer width="100%" height={280}>
            <LineChart data={debtEvolutionData}>
              <CartesianGrid
                strokeDasharray="3 3"
                stroke="hsl(220, 13%, 89%)"
              />
              <XAxis
                dataKey="month"
                tick={{ fontSize: 12 }}
                stroke="hsl(220, 10%, 46%)"
              />
              <YAxis
                tick={{ fontSize: 12 }}
                stroke="hsl(220, 10%, 46%)"
                tickFormatter={(v) => `$${(v / 1000).toFixed(0)}k`}
              />
              <Tooltip formatter={(v: number) => [`$${v.toLocaleString()}`, "Dette totale"]} />
              <Line
                type="monotone"
                dataKey="total"
                stroke="hsl(239, 84%, 67%)"
                strokeWidth={3}
                dot={{ fill: "hsl(239, 84%, 67%)", r: 4 }}
              />
            </LineChart>
          </ResponsiveContainer>
        </Card>

        <Card className="p-5 rounded-2xl shadow-card bg-card/90">
          <div className="mb-4">
            <h3 className="font-display font-semibold text-card-foreground">
              Répartition par type
            </h3>
            <p className="text-sm text-muted-foreground">
              Catégories de dettes les plus lourdes.
            </p>
          </div>

          {debtByTypeData.length > 0 ? (
            <>
              <ResponsiveContainer width="100%" height={220}>
                <PieChart>
                  <Pie
                    data={debtByTypeData}
                    dataKey="value"
                    nameKey="name"
                    cx="50%"
                    cy="50%"
                    outerRadius={82}
                    innerRadius={50}
                    strokeWidth={0}
                  >
                    {debtByTypeData.map((entry, i) => (
                      <Cell key={i} fill={entry.fill} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(v: number) => `$${v.toLocaleString()}`} />
                </PieChart>
              </ResponsiveContainer>
              <div className="space-y-3 mt-3">
                {debtByTypeData.map((d) => (
                  <div key={d.name} className="flex items-center gap-2 text-sm rounded-xl bg-muted/20 px-3 py-2">
                    <div className="w-3 h-3 rounded-full" style={{ background: d.fill }} />
                    <span className="text-muted-foreground">{d.name}</span>
                    <span className="ml-auto font-medium text-card-foreground">${d.value.toLocaleString()}</span>
                  </div>
                ))}
              </div>
            </>
          ) : (
            <p className="text-sm text-muted-foreground text-center py-10">Aucune dette enregistrée.</p>
          )}
        </Card>
      </div>

      {/* Payment section */}
      <Card className="p-5 rounded-2xl shadow-card bg-card/90">
        <div className="flex items-center justify-between mb-4">
          <div>
            <h3 className="font-display font-semibold text-card-foreground">
              Paiements mensuels
            </h3>
            <p className="text-sm text-muted-foreground">
              Suis ton effort de remboursement mois après mois.
            </p>
          </div>
          <Badge variant="outline" className="rounded-full">
            Historique récent
          </Badge>
        </div>

        {paymentHistoryData.length > 0 ? (
          <ResponsiveContainer width="100%" height={240}>
            <BarChart data={paymentHistoryData}>
              <CartesianGrid strokeDasharray="3 3" stroke="hsl(220, 13%, 89%)" />
              <XAxis dataKey="month" tick={{ fontSize: 12 }} stroke="hsl(220, 10%, 46%)" />
              <YAxis tick={{ fontSize: 12 }} stroke="hsl(220, 10%, 46%)" tickFormatter={(v) => `$${v}`} />
              <Tooltip formatter={(v: number) => [`$${v.toLocaleString()}`, "Montant payé"]} />
              <Bar dataKey="paid" fill="hsl(142, 71%, 45%)" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        ) : (
          <p className="text-sm text-muted-foreground text-center py-10">
            Aucun paiement enregistré pour le moment.
          </p>
        )}
      </Card>

      {/* Strategy / action reminder */}
      <div className="grid lg:grid-cols-3 gap-6">
        <Card className="lg:col-span-1 p-5 rounded-2xl shadow-card bg-card/90">
          <div className="flex items-center gap-2 mb-3">
            <Target className="h-4 w-4 text-primary" />
            <h3 className="font-display font-semibold text-card-foreground">
              Plan actuel
            </h3>
          </div>

          <div className="space-y-4">
            <div className="rounded-xl bg-muted/20 p-4">
              <p className="text-xs text-muted-foreground mb-1">Stratégie active</p>
              <p className="font-display text-lg font-bold text-card-foreground">
                {localStorage.getItem("strategy") === "1" ? "Snowball" : "Avalanche"}
              </p>
            </div>

            {highestRateDebt && (
              <div className="rounded-xl bg-muted/20 p-4">
                <p className="text-xs text-muted-foreground mb-1">Priorité actuelle</p>
                <p className="font-medium text-card-foreground">{highestRateDebt.creditor}</p>
                <p className="text-xs text-muted-foreground mt-1">
                  {highestRateDebt.interestRate}% d’intérêt
                </p>
              </div>
            )}

            <div className="rounded-xl bg-primary/5 border border-primary/10 p-4">
              <p className="text-sm text-muted-foreground">
                En gardant cette stratégie, tu minimises le coût des intérêts sur
                le long terme.
              </p>
            </div>
          </div>
        </Card>

        <Card className="lg:col-span-2 p-5 rounded-2xl shadow-card bg-card/90">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h3 className="font-display font-semibold text-card-foreground">
                Progression par dette
              </h3>
              <p className="text-sm text-muted-foreground">
                Identifie les dettes déjà avancées et celles à surveiller.
              </p>
            </div>
            <Badge variant="outline" className="rounded-full">
              Vue détaillée
            </Badge>
          </div>

          {activeDebts.length > 0 ? (
            <div className="space-y-5">
              {activeDebts.map((debt) => {
                const pct = debt.originalAmount > 0
                  ? Math.round(((debt.originalAmount - debt.remainingAmount) / debt.originalAmount) * 100)
                  : 0;
                return (
                  <div key={debt.id} className="space-y-2">
                    <div className="flex items-center justify-between gap-4 text-sm">
                      <div>
                        <span className="font-medium text-card-foreground">{debt.creditor}</span>
                        <p className="text-xs text-muted-foreground mt-0.5">
                          Taux : {debt.interestRate}%
                        </p>
                      </div>
                      <span className="text-muted-foreground text-right">
                        ${debt.remainingAmount.toLocaleString()} restants • {pct}%
                      </span>
                    </div>
                    <Progress value={pct} className="h-2.5" />
                  </div>
                );
              })}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground text-center py-10">
              Aucune dette active. Ajoutez vos dettes pour commencer le suivi.
            </p>
          )}
        </Card>
      </div>

      {/* Bottom callout */}
      <Card className="p-5 rounded-2xl border bg-primary/5 border-primary/10 shadow-card">
        <div className="flex items-start gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
            <ArrowRight className="h-5 w-5 text-primary" />
          </div>

          <div>
            <h3 className="font-display text-lg font-semibold text-foreground">
              Prochaine bonne action
            </h3>
            <p className="text-sm text-muted-foreground mt-1">
              Enregistre ton prochain paiement ou consulte le simulateur de stratégie
              pour voir si un budget supplémentaire pourrait accélérer ta sortie de dette.
            </p>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default Dashboard;