import { useState, useEffect } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Plus, Receipt, CheckCircle2 } from "lucide-react";
import { toast } from "sonner";
import { getDebts } from "@/apis/DebtApi";
import { createPayment, getPayments } from "@/apis/PaymentApi";

const Payments = () => {
  const [payments, setPayments] = useState<any[]>([]);
  const [debts, setDebts] = useState<any[]>([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ debtId: '', amount: '', date: '', notes: '' });
  const [saving, setSaving] = useState(false);

  async function loadAll() {
    const [p, d] = await Promise.all([getPayments(), getDebts()]);
    setPayments(p ?? []);
    setDebts(d ?? []);
  }

  useEffect(() => {
    loadAll();
  }, []);

  const handleOpenDialog = () => {
    setForm({ debtId: '', amount: '', date: '', notes: '' });
    setOpen(true);
  };

  const handleAdd = async () => {
    if (!form.debtId || !form.amount) {
      toast.error("Veuillez sélectionner une dette et saisir un montant.");
      return;
    }

    const amount = parseFloat(form.amount);
    const selectedDebt = debts.find(d => String(d.id) === form.debtId);

    if (selectedDebt && amount > (selectedDebt.remainingAmount ?? selectedDebt.originalAmount)) {
      toast.error("Le montant dépasse le solde restant de cette dette.");
      return;
    }

    setSaving(true);
    try {
      const result = await createPayment({
        debtId: Number(form.debtId),
        amount,
        paymentDate: form.date ? new Date(form.date).toISOString() : new Date().toISOString(),
        notes: form.notes || undefined,
      });

      if (result) {
        toast.success("Paiement enregistré !", {
          description: "Votre solde a été mis à jour.",
          icon: <CheckCircle2 className="h-4 w-4" />,
        });
        await loadAll();
        setOpen(false);
        setForm({ debtId: '', amount: '', date: '', notes: '' });
      } else {
        toast.error("Erreur lors de l'enregistrement du paiement.");
      }
    } finally {
      setSaving(false);
    }
  };

  // Retrouver le nom du créditeur depuis la liste locale des dettes
  const getCreditor = (debtId: number) =>
    debts.find(d => d.id === debtId)?.creditor ?? `Dette #${debtId}`;

  const totalPaid = payments.reduce((s, p) => s + (p.amount ?? 0), 0);
  const activeDebts = debts.filter(d => d.status === 0);

  return (
    <div className="space-y-6 max-w-5xl mx-auto animate-fade-in">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-foreground">Paiements</h1>
          <p className="text-muted-foreground text-sm">Enregistrez et suivez vos remboursements</p>
        </div>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button className="gradient-primary border-0" onClick={handleOpenDialog}>
              <Plus className="h-4 w-4 mr-2" /> Nouveau paiement
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="font-display">Ajouter un paiement</DialogTitle>
            </DialogHeader>
            <div className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label>Dette</Label>
                <Select value={form.debtId} onValueChange={v => setForm({ ...form, debtId: v })}>
                  <SelectTrigger>
                    <SelectValue placeholder="Sélectionner la dette à rembourser" />
                  </SelectTrigger>
                  <SelectContent>
                    {activeDebts.length === 0 && (
                      <SelectItem value="__none" disabled>Aucune dette active</SelectItem>
                    )}
                    {activeDebts.map(d => (
                      <SelectItem key={d.id} value={String(d.id)}>
                        {d.creditor} — Restant : {d.remainingAmount ?? d.originalAmount}€ ({d.interestRate}%)
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Montant (€)</Label>
                  <Input
                    type="number"
                    min="0.01"
                    step="0.01"
                    value={form.amount}
                    onChange={e => setForm({ ...form, amount: e.target.value })}
                    placeholder="500"
                  />
                </div>
                <div className="space-y-2">
                  <Label>Date</Label>
                  <Input
                    type="date"
                    value={form.date}
                    onChange={e => setForm({ ...form, date: e.target.value })}
                  />
                </div>
              </div>
              <div className="space-y-2">
                <Label>Commentaire (optionnel)</Label>
                <Input
                  value={form.notes}
                  onChange={e => setForm({ ...form, notes: e.target.value })}
                  placeholder="Note optionnelle"
                />
              </div>
              <Button
                className="w-full gradient-primary border-0"
                onClick={handleAdd}
                disabled={saving || !form.debtId || !form.amount}
              >
                {saving ? "Enregistrement…" : "Valider"}
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      {/* KPIs */}
      <div className="grid sm:grid-cols-2 gap-4">
        <Card className="p-5 shadow-card">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-success/10 flex items-center justify-center">
              <Receipt className="h-5 w-5 text-success" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total remboursé</p>
              <p className="text-xl font-display font-bold text-card-foreground">
                {totalPaid.toLocaleString("fr-FR", { style: "currency", currency: "EUR" })}
              </p>
            </div>
          </div>
        </Card>
        <Card className="p-5 shadow-card">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center">
              <CheckCircle2 className="h-5 w-5 text-primary" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Paiements effectués</p>
              <p className="text-xl font-display font-bold text-card-foreground">{payments.length}</p>
            </div>
          </div>
        </Card>
      </div>

      {/* Table */}
      <Card className="shadow-card overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Date</TableHead>
              <TableHead>Créditeur</TableHead>
              <TableHead className="text-right">Montant</TableHead>
              <TableHead>Notes</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {payments.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} className="text-center text-muted-foreground py-8">
                  Aucun paiement enregistré.
                </TableCell>
              </TableRow>
            )}
            {payments.map((p) => {
              const date = p.paymentDate ? new Date(p.paymentDate).toLocaleDateString("fr-FR") : "—";
              const creditor = p?.debt?.creditor ?? getCreditor(p.debtId);
              return (
                <TableRow key={p.id}>
                  <TableCell>{date}</TableCell>
                  <TableCell className="font-medium">{creditor}</TableCell>
                  <TableCell className="text-right">
                    <Badge variant="secondary">
                      {(p.amount ?? 0).toLocaleString("fr-FR", { style: "currency", currency: "EUR" })}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-muted-foreground text-sm">{p.notes ?? "—"}</TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </Card>
    </div>
  );
};

export default Payments;
