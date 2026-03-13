import { useState, useEffect } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Plus, CreditCard, GraduationCap, Banknote } from "lucide-react";
import { toast } from "sonner";
import { DebtCreateUpdateDto } from "@/dtos/DebtCreateDto";
import { getDebts, createDebt } from "@/apis/DebtApi";

// Mapping enum backend (0=CREDIT_CARD, 1=PERSONAL_LOAN, 2=STUDENT_LOAN)
const typeKeyMap: Record<number, string> = {
  0: 'credit-card',
  1: 'personal-loan',
  2: 'student-loan',
};

const typeIcons: Record<string, React.ElementType> = {
  'credit-card': CreditCard,
  'student-loan': GraduationCap,
  'personal-loan': Banknote,
};

const typeLabels: Record<string, string> = {
  'credit-card': 'Carte crédit',
  'student-loan': 'Crédit étudiant',
  'personal-loan': 'Crédit personnel',
};

const emptyForm: Partial<DebtCreateUpdateDto> = {
  creditor: "",
  originalAmount: 0,
  interestRate: 0,
  dueDate: "",
  type: 0,
};

const Debts = () => {
  const [debts, setDebts] = useState<any[]>([]);
  const [editingDebt, setEditingDebt] = useState<Partial<DebtCreateUpdateDto>>(emptyForm);
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);

  async function loadDebts() {
    const data = await getDebts();
    setDebts(data ?? []);
  }

  useEffect(() => {
    loadDebts();
  }, []);

  const handleOpenCreate = () => {
    setEditingDebt(emptyForm);
    setOpen(true);
  };

  const handleSave = async () => {
    if (!editingDebt?.creditor?.trim()) {
      toast.error("Le nom du créditeur est obligatoire.");
      return;
    }
    if (!editingDebt.dueDate) {
      toast.error("La date de fin est obligatoire.");
      return;
    }

    setSaving(true);
    try {
      const result = await createDebt({
        creditor: editingDebt.creditor.trim(),
        originalAmount: Number(editingDebt.originalAmount) || 0,
        interestRate: Number(editingDebt.interestRate) || 0,
        dueDate: editingDebt.dueDate,
        type: Number(editingDebt.type) ?? 0,
      });

      if (result) {
        toast.success("Dette ajoutée avec succès !");
        await loadDebts();
        setOpen(false);
        setEditingDebt(emptyForm);
      } else {
        toast.error("Erreur lors de l'ajout de la dette. Vérifiez que vous êtes connecté.");
      }
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-6 max-w-7xl mx-auto animate-fade-in">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-foreground">Dettes</h1>
          <p className="text-muted-foreground text-sm">Gérez toutes vos dettes au même endroit</p>
        </div>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button className="gradient-primary border-0" onClick={handleOpenCreate}>
              <Plus className="h-4 w-4 mr-2" /> Ajouter une dette
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="font-display">Ajouter une nouvelle dette</DialogTitle>
            </DialogHeader>
            <div className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label>Créditeur</Label>
                <Input
                  value={editingDebt?.creditor || ''}
                  onChange={e => setEditingDebt({ ...editingDebt, creditor: e.target.value })}
                  placeholder="ex. Crédit Agricole"
                />
              </div>
              <div className="space-y-2">
                <Label>Montant original (€)</Label>
                <Input
                  type="number"
                  min="0"
                  step="0.01"
                  value={editingDebt?.originalAmount || 0}
                  onChange={e => setEditingDebt({ ...editingDebt, originalAmount: parseFloat(e.target.value) || 0 })}
                />
              </div>
              <div className="space-y-2">
                <Label>Taux d'intérêt (%)</Label>
                <Input
                  type="number"
                  step="0.01"
                  min="0"
                  value={editingDebt?.interestRate || 0}
                  onChange={e => setEditingDebt({ ...editingDebt, interestRate: parseFloat(e.target.value) || 0 })}
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Date de fin</Label>
                  <Input
                    type="date"
                    value={editingDebt?.dueDate || ''}
                    onChange={e => setEditingDebt({ ...editingDebt, dueDate: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label>Type</Label>
                  <Select
                    value={String(editingDebt?.type ?? 0)}
                    onValueChange={v => setEditingDebt({ ...editingDebt, type: Number(v) })}
                  >
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>
                      <SelectItem value="0">Carte crédit</SelectItem>
                      <SelectItem value="1">Crédit personnel</SelectItem>
                      <SelectItem value="2">Crédit étudiant</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <Button
                className="w-full gradient-primary border-0"
                onClick={handleSave}
                disabled={saving}
              >
                {saving ? "Enregistrement…" : "Enregistrer"}
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      {/* Cards view */}
      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {[...debts].reverse().map((debt) => {
          const remaining = debt.remainingAmount ?? debt.originalAmount;
          const pct = debt.originalAmount > 0
            ? Math.round(((debt.originalAmount - remaining) / debt.originalAmount) * 100)
            : 0;
          const typeKey = typeKeyMap[debt.type as number] ?? 'credit-card';
          const Icon = typeIcons[typeKey] ?? CreditCard;
          return (
            <Card key={debt.id} className="p-5 shadow-card hover:shadow-elevated transition-shadow">
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center">
                    <Icon className="h-5 w-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-semibold text-card-foreground">{debt.creditor}</p>
                    <Badge variant="secondary" className="text-xs mt-0.5">{typeLabels[typeKey]}</Badge>
                  </div>
                </div>
              </div>
              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Montant restant</span>
                  <span className="font-semibold text-card-foreground">{remaining}€</span>
                </div>
                <Progress value={Math.min(pct, 100)} className="h-2" />
                <div className="flex justify-between text-xs text-muted-foreground">
                  <span>{pct}% Payé</span>
                  <span>{debt.interestRate}% Intérêt</span>
                </div>
              </div>
            </Card>
          );
        })}
      </div>

      {debts.length === 0 && (
        <Card className="p-10 text-center shadow-card">
          <p className="text-muted-foreground">Aucune dette enregistrée. Ajoutez votre première dette.</p>
        </Card>
      )}

      {/* Table view */}
      <Card className="shadow-card overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Créditeur</TableHead>
              <TableHead>Type</TableHead>
              <TableHead className="text-right">Montant original</TableHead>
              <TableHead className="text-right">Montant restant</TableHead>
              <TableHead className="text-right">Intérêt</TableHead>
              <TableHead className="text-right">Date de fin</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {debts.map((debt) => {
              const remaining = debt.remainingAmount ?? debt.originalAmount;
              const typeKey = typeKeyMap[debt.type as number] ?? 'credit-card';
              const dueDate = debt.dueDate ? new Date(debt.dueDate).toLocaleDateString("fr-FR") : "—";
              return (
                <TableRow key={debt.id}>
                  <TableCell className="font-medium">{debt.creditor}</TableCell>
                  <TableCell>
                    <Badge variant="outline" className="text-xs">{typeLabels[typeKey]}</Badge>
                  </TableCell>
                  <TableCell className="text-right">{debt.originalAmount}€</TableCell>
                  <TableCell className="text-right">{remaining}€</TableCell>
                  <TableCell className="text-right">{debt.interestRate}%</TableCell>
                  <TableCell className="text-right">{dueDate}</TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </Card>
    </div>
  );
};

export default Debts;
