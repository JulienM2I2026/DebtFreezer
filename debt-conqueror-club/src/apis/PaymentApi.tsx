export async function getPayments() {
    const token = localStorage.getItem("token");
    try {
        const response = await fetch("http://localhost:5099/api/Payment", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        });

        const data = await response.json(); // lire UNE seule fois

        if (!response.ok) {
        console.log("Failed to load Payments", data);
        return; 
        }
        
        return data;
    } catch (error) {
        console.log("Failed to load Payments", error);
    }
}

export async function createDebt(payload) {
    console.log("payload: ", payload)
    const token = localStorage.getItem("token");
    try {
        const response = await fetch("http://localhost:5099/api/Payment", {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(payload),
    });

    const data = await response.json(); // lire UNE seule fois
    console.log("tralala: ",data )
    if (!response.ok) {
        console.log("Failed to create Payment", data);
        return;
    }
    return data
    } catch (error) {
        console.log("Failed to create Payment", error);
    }
}