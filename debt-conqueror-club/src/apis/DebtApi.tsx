  export async function getDebts() {
    const token = localStorage.getItem("token");
    try {
        const response = await fetch("http://localhost:5099/api/Debt", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
      });

      const data = await response.json(); // lire UNE seule fois

      if (!response.ok) {
        console.log("Failed to load Debts", data);
        return;
      }
      
      return data;
    } catch (error) {
      console.log("Failed to load Debts", error);
    }
  }