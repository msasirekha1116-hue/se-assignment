const api_url = "http://localhost:10010";

export const startPlan = async () => {
    const url = `${api_url}/Plan`;
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify({}),
    });

    if (!response.ok) throw new Error("Failed to create plan");

    return await response.json();
};

export const addProcedureToPlan = async (planId, procedureId) => {
    const url = `${api_url}/Plan/AddProcedureToPlan`;
    var command = { planId: planId, procedureId: procedureId };
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(command),
    });

    if (!response.ok) throw new Error("Failed to create plan");

    return true;
};

export const getProcedures = async () => {
    const url = `${api_url}/Procedures`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get procedures");

    return await response.json();
};

export const getPlanProcedures = async (planId) => {
    const url = `${api_url}/PlanProcedure?$filter=planId eq ${planId}&$expand=procedure`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get plan procedures");

    return await response.json();
};

export const getUsers = async () => {
    const url = `${api_url}/Users`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get users");

    return await response.json();
};
export const assignUser = async ({ planId, procedureId, userId }) => {
  try {
    const response = await fetch(`/api/assignments/assign`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ planId, procedureId, userId }),
    });

    if (!response.ok) {
      throw new Error('Failed to assign user');
    }

    return true;
  } catch (error) {
    console.error(error);
    throw error;
  }
};
export const removeUser = async ({ planId, procedureId, userId }) => {
  try {
    const response = await fetch(`/api/assignments/remove`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ planId, procedureId, userId }),
    });

    if (!response.ok) {
      throw new Error('Failed to remove user');
    }

    return true;
  } catch (error) {
    console.error(error);
    throw error;
  }
};
export const clearUsers = async ({ planId, procedureId }) => {
  try {
    const response = await fetch(`/api/assignments/clear`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ planId, procedureId }),
    });

    if (!response.ok) {
      throw new Error('Failed to clear users');
    }
    return true;
  } catch (error) {
    console.error(error);
    throw error;
  }
};
export const getPlanAssignments = async (planId) => {
    const url = `${api_url}/api/assignments/${planId}`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get plan procedures");

    return await response.json();
};