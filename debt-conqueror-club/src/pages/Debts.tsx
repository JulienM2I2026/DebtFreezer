import { useState, useEffect } from "react";
import { mockDebts, type Debt } from "@/lib/mockData";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Plus, Pencil, Trash2, CreditCard, GraduationCap, Banknote } from "lucide-react";
import { toast } from "sonner";
import { DebtCreateUpdateDto, DebtType } from "@/dtos/DebtCreateDto";
import { getDebts } from "@/apis/DebtApi";

const typeIcons = {
  'credit-card': CreditCard,
  'student-loan': GraduationCap,
  'personal-loan': Banknote,
};

const typeLabels = {
  'credit-card': 'Credit Carte',
  'student-loan': 'Emprunt Etudiant',
  'personal-loan': 'Emprunt Personnel',
};

const typeKeyMap = {
  0: 'credit-card',
  1: 'personal-loan',
  2: 'student-loan',
};

const emptyDebt: Omit<Debt, 'id'> = {
  creditor: '', originalAmount: 0, remainingAmount: 0, interestRate: 0,
  dueDate: '', type: 'credit-card', minPayment: 0,
};

const Debts = () => {
  //const [debts, setDebts] = useState<Debt[]>(mockDebts);
  const [debts, setDebts] = useState([]);
  const [editingDebt, setEditingDebt] = useState<Partial<DebtCreateUpdateDto> | null>({
    creditor: "",
    originalAmount: 0,
    interestRate: 0,
    dueDate: "",
    type: 0
  });
  const [open, setOpen] = useState(false);
  

  async function getAllDebts() {
    const data = await getDebts();
    setDebts(data)
  }

  async function createDebt() {
    console.log("editingDebt: ", editingDebt)
    const token = localStorage.getItem("token");
    const response = await fetch("http://localhost:5099/api/Debt", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify(editingDebt),
    });

    const data = await response.json(); // lire UNE seule fois

    if (!response.ok) {
      console.log("Failed to load Debts", data);
      return;
    }
    setDebts([...debts, data])
  }

  useEffect(() => {
    getAllDebts()
  }, [])

  const handleChange = (type, value) => {
    if (["originalAmount", "interestRate"].includes(type)) {
      value = value.replace(/[^0-9]/g, "");
    }
    setEditingDebt({ ...editingDebt, [type]: value });
  };



  const handleSave = () => {
    if (!editingDebt?.creditor) return;
    console.log("editingDebt: ", editingDebt);
    createDebt()
    //if (editingDebt.id) {
    //  setDebts(debts.map(d => d.id === editingDebt.id ? { ...d, ...editingDebt } as Debt : d));
    //  toast.success('Debt updated successfully!');
    //} else {
    //  const newDebt: Debt = { ...emptyDebt, ...editingDebt, id: Date.now().toString() } as Debt;
    //  setDebts([...debts, newDebt]);
    //  toast.success('Debt added successfully!');
    //}
    setOpen(false);
    setEditingDebt(null);
  };

  const handleDelete = (id: string) => {
    setDebts(debts.filter(d => d.id !== id));
    toast.success('Debt removed');
  };

  return (
    <div className="space-y-6 max-w-7xl mx-auto animate-fade-in">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-foreground">Dette</h1>
          <p className="text-muted-foreground text-sm">Gérez toutes vos dettes au même endroit</p>
        </div>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button className="gradient-primary border-0" onClick={() => setEditingDebt({})}>
              <Plus className="h-4 w-4 mr-2" /> Add Debt
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="font-display">{editingDebt?.id ? 'Edit Debt' : 'Add New Debt'}</DialogTitle>
            </DialogHeader>
            <div className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label>Crédit</Label>
                <Input value={editingDebt?.creditor || ''} onChange={e => setEditingDebt({ ...editingDebt, creditor: e.target.value })} placeholder="e.g. Chase Sapphire" />
              </div>
              <div className="grid gap-4">
                <div className="space-y-2">
                  <Label>Montant du crédit</Label>
                  <Input type="number" value={editingDebt?.originalAmount || 0} onChange={e => handleChange("originalAmount", e.target.value)} />
                </div>
              </div>
              <div className="grid gap-4">
                <div className="space-y-2">
                  <Label>Intérêt (%)</Label>
                  <Input type="number" step="0.01" value={editingDebt?.interestRate || 0} onChange={e => setEditingDebt({ ...editingDebt, interestRate: + e.target.value.replace(/[^0-9]/g, '') })} />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Date de fin</Label>
                  <Input type="date" value={editingDebt?.dueDate || ''} onChange={e => setEditingDebt({ ...editingDebt, dueDate: e.target.value })} />
                </div>
                <div className="space-y-2">
                  <Label>Type</Label>
                  <Select value={editingDebt?.type || "0" } onValueChange={v => setEditingDebt({ ...editingDebt, type: v  })}>
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>
                      <SelectItem value="0">Carte crédit</SelectItem>
                      <SelectItem value="1">Crédit personnel</SelectItem>
                      <SelectItem value="2">Crédit étudiant</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <Button className="w-full gradient-primary border-0" onClick={handleSave}>Save Debt</Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      {/* Cards view */}
      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {debts?.length > 0 && debts?.reverse()?.map((debt) => {
          const pct = Math.round(((debt.originalAmount - debt.remainingAmount) / debt.originalAmount) * 100);
          const Icon = typeIcons[typeKeyMap[debt.type]];
          return (
            <Card key={debt.id} className="p-5 shadow-card hover:shadow-elevated transition-shadow">
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center">
                    <Icon className="h-5 w-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-semibold text-card-foreground">{debt.creditor}</p>
                    <Badge variant="secondary" className="text-xs mt-0.5">{typeLabels[typeKeyMap[debt.type]]}</Badge>
                  </div>
                </div>
                <div className="flex gap-1">
                  <Button variant="ghost" size="icon" className="h-8 w-8" onClick={() => { setEditingDebt(debt); setOpen(true); }}>
                    <Pencil className="h-3.5 w-3.5" />
                  </Button>
                  <Button variant="ghost" size="icon" className="h-8 w-8 text-destructive" onClick={() => handleDelete(debt.id)}>
                    <Trash2 className="h-3.5 w-3.5" />
                  </Button>
                </div>
              </div>
              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Montant restant</span>
                  <span className="font-semibold text-card-foreground">{debt.remainingAmount}€</span>
                </div>
                <Progress value={pct} className="h-2" />
                <div className="flex justify-between text-xs text-muted-foreground">
                  <span>{pct}% Payé</span>
                  <span>{debt.interestRate}% Intérêt</span>
                </div>
              </div>
            </Card>
          );
        })}
      </div>

      {/* Table view */}
      <Card className="shadow-card overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Crédit</TableHead>
              <TableHead>Type de crédit</TableHead>
              <TableHead className="text-right">Montant original</TableHead>
              <TableHead className="text-right">Montant restant à payer</TableHead>
              <TableHead className="text-right">Intérêt</TableHead>
              <TableHead className="text-right">Date de fin</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {debts?.length > 0 && debts.map((debt) => {
                const dueDate = new Date(debt.dueDate).toLocaleDateString();
                return(
                <TableRow key={debt.id}>
                  <TableCell className="font-medium">{debt.creditor}</TableCell>
                  <TableCell><Badge variant="outline" className="text-xs">{typeLabels[typeKeyMap[debt.type]]}</Badge></TableCell>
                  <TableCell className="text-right">{debt.originalAmount}€</TableCell>
                  <TableCell className="text-right">{debt.remainingAmount}€</TableCell>
                  <TableCell className="text-right">{debt.interestRate}%</TableCell>
                  <TableCell className="text-right">{dueDate}</TableCell>
                </TableRow>
                )
              }
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  );
};

export default Debts;
