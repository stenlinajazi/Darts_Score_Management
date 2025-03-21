export const BASE_URL = "https://localhost:7134/api";

export const fetchGames = async () => {
  const response = await fetch(`${BASE_URL}/Games`, {
    method: "GET",
    headers: { "Content-Type": "application/json" },
  });
  if (!response.ok) throw new Error("Failed to fetch games list");
  return response.json();
};

export const fetchGameDetails = async (gameId) => {
  const response = await fetch(`${BASE_URL}/Games/${gameId}`, {
    method: "GET",
    headers: { "Content-Type": "application/json" },
  });
  if (!response.ok) throw new Error("Failed to fetch game details");
  return response.json();
};

export const createGame = async (gameData) => {
  const response = await fetch(`${BASE_URL}/Games`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(gameData),
  });
  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || "Failed to create game");
  }
  return response.json();
};

export const submitThrows = async (gameId, throws) => {
  const response = await fetch(
    `${BASE_URL}/GameRules/throws?gameId=${gameId}`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(throws),
    }
  );
  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || "Failed to submit turn");
  }
  return response.json();
};

export const deleteGame = async (gameId) => {
  const response = await fetch(`${BASE_URL}/Games/${gameId}`, {
    method: "DELETE",
    headers: { "Content-Type": "application/json" },
  });
  return handleResponse(response);
};

export const getPlayers = async () => {
  const response = await fetch(`${BASE_URL}/Players`);
  return handleResponse(response);
};

export const createPlayer = async (playerData) => {
  const response = await fetch(`${BASE_URL}/Players`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(playerData),
  });
  return handleResponse(response);
};

export const updatePlayer = async (playerId, playerData) => {
  const response = await fetch(`${BASE_URL}/Players/${playerId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(playerData),
  });
  return handleResponse(response);
};

export const deletePlayer = async (playerId) => {
  const response = await fetch(`${BASE_URL}/Players/${playerId}`, {
    method: "DELETE",
  });
  return handleResponse(response);
};

export const getPlayerStats = async (playerId) => {
  const response = await fetch(`${BASE_URL}/PlayerStats/${playerId}/stats`);
  return handleResponse(response);
};

const handleResponse = async (response) => {
  if (!response.ok) {
    let errorMessage = response.statusText;
    try {
      const errorData = await response.json();
      errorMessage = errorData.message || errorData.error || errorMessage;
    } catch (e) {
      console.warn("Failed to parse error response:", e);
    }
    throw new Error(`${response.status}: ${errorMessage}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
};
