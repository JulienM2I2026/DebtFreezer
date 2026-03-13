import { useState, useEffect } from "react";
import { mockPayments, mockDebts } from "@/lib/mockData";
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
import { createDebt, getPayments } from "@/apis/PaymentApi";
import { log } from "console";

const Payments = () => {
  const [payments, setPayments] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ debtId: '', amount: '', date: '', notes: '' });
  const [debts, setDebts] = useState([])

  const loadDebtsForPaymentCreate = async () => {
    if(debts.length > 0) return
    setDebts(await getDebts())
  }

  const getAllPayment = async () => {
    const data = await getPayments()
    setPayments(data)
  }

  const handleAdd = async () => {
    console.log("je suis la: ", form)
    const data = await createDebt(form)
    console.log("data apres create: ", data)
    //if (!form.debtId || !form.amount) return;
    //setPayments([{ id: Date.now().toString(), debtId: form.debtId, amount: +form.amount, date: form.date || new Date().toISOString().split('T')[0], notes: form.notes }, ...payments]);
    //toast.success('Payment recorded!', { description: 'Your balance has been updated.', icon: <CheckCircle2 className="h-4 w-4" /> });
    //setOpen(false);
    //setForm({ debtId: '', amount: '', date: '', notes: '' });
  };

  useEffect(() => {
    getAllPayment()
  }, [])


    useEffect(() => {
    console.log("payments:", payments)
  }, [payments])

  const getCreditor = (debtId: string) => mockDebts.find(d => d.id === debtId)?.creditor || 'Unknown';
  const totalPaid = payments.reduce((s, p) => s + p.amount, 0);

  useEffect(() => {
    console.log(debts)
  }, [debts])

  return (
    <div className="space-y-6 max-w-5xl mx-auto animate-fade-in">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-foreground">Payments</h1>
          <p className="text-muted-foreground text-sm">Track and record your debt payments</p>
        </div>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button className="gradient-primary border-0" onClick={() => loadDebtsForPaymentCreate()}><Plus className="h-4 w-4 mr-2" /> Nouveau paiement</Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="font-display">Ajouter un paiement</DialogTitle>
            </DialogHeader>
            <div className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label>Dette</Label>
                <Select value={form.debtId} onValueChange={v => setForm({ ...form, debtId: v })}>
                  <SelectTrigger><SelectValue placeholder="Selectionner la dette à payer" /></SelectTrigger>
                  <SelectContent>
                    {debts?.filter(debt => debt.status == 0)?.map(d => <SelectItem key={d.id} value={d.id}>{d.creditor} - (Montant restant: {d.remainingAmount}, Intérêt: {d.interestRate}%)</SelectItem>)}
                  </SelectContent>
                </Select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Montant (€)</Label>
                  <Input type="number" value={form.amount} onChange={e => setForm({ ...form, amount: e.target.value })} placeholder="500" />
                </div>
                <div className="space-y-2">
                  <Label>Date</Label>
                  <Input type="date" value={form.date} onChange={e => setForm({ ...form, date: e.target.value })} />
                </div>
              </div>
              <div className="space-y-2">
                <Label>Commentaire</Label>
                <Input value={form.notes} onChange={e => setForm({ ...form, notes: e.target.value })} placeholder="Commentaire (Optionnel)" />
              </div>
              <Button className="w-full gradient-primary border-0" onClick={handleAdd} disabled={!form.debtId || !form.amount || !form.date || debts?.find(d => d.id == form.debtId)?.remainingAmount < form?.amount } >Valider</Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      <div className="grid sm:grid-cols-2 gap-4">
        <Card className="p-5 shadow-card">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-success/10 flex items-center justify-center">
              <Receipt className="h-5 w-5 text-success" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Payments</p>
              <p className="text-xl font-display font-bold text-card-foreground">${totalPaid.toLocaleString()}</p>
            </div>
          </div>
        </Card>
        <Card className="p-5 shadow-card">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center">
              <CheckCircle2 className="h-5 w-5 text-primary" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Payments Made</p>
              <p className="text-xl font-display font-bold text-card-foreground">{payments.length}</p>
            </div>
          </div>
        </Card>
      </div>

      <Card className="shadow-card overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Date</TableHead>
              <TableHead>Creditor</TableHead>
              <TableHead className="text-right">Amount</TableHead>
              <TableHead>Notes</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {payments.map((p) => {
              const date = new Date(p.paymentDate).toLocaleDateString();
              return(
              <TableRow key={p.id}>
                <TableCell>{date}</TableCell>
                <TableCell className="font-medium">{p?.debt?.creditor}</TableCell>
                <TableCell className="text-right"><Badge variant="secondary">${p.amount.toLocaleString()}</Badge></TableCell>
                <TableCell className="text-muted-foreground text-sm">{p.notes}</TableCell>
              </TableRow>
            )}
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  );
};

export default Payments;
