const SEGMENTS = {
  MISS: 0,
  BULL: 25,
};

const MULTIPLIERS = {
  SINGLE: 1,
  DOUBLE: 2,
  TRIPLE: 3,
};

const uiState = {
  gameId: null,
  startingScore: null,
  players: [],
  activePlayerIndex: 0,
  selectedSegment: null,
  selectedMultiplier: null,
  currentThrows: [],
  apiBaseUrl: "https://localhost:7134/api",
};

const createGameBtn = document.getElementById("create-game-btn");
const playersContainer = document.getElementById("players-container");
const throwsList = document.getElementById("throws-list");
const submitTurnBtn = document.getElementById("submit-turn-btn");
const clearBtn = document.getElementById("clear-btn");
const backBtn = document.getElementById("back-btn");
const segmentButtons = document.querySelectorAll(".segment-btn");
const multiplierButtons = document.querySelectorAll(".multiplier-btn");
const messageContainer = document.getElementById("message-container");
const winnerModal = document.getElementById("winner-modal");
const modalTitle = document.getElementById("modal-title");
const modalMessage = document.getElementById("modal-message");
const modalClose = document.getElementById("modal-close");
const modalOk = document.getElementById("modal-ok");

const createGameModal = document.getElementById("create-game-modal");
const createModalClose = document.getElementById("create-modal-close");
const createGameSubmit = document.getElementById("create-game-submit");
const playerIdsInput = document.getElementById("player-ids");
const startingScoreSelect = document.getElementById("starting-score");
const setsToWinInput = document.getElementById("sets-to-win");
const legsPerSetInput = document.getElementById("legs-per-set");
const mustFinishOnDoubleCheckbox = document.getElementById(
  "must-finish-on-double"
);

function init() {
  createGameBtn.addEventListener("click", () => {
    createGameModal.style.display = "block";
  });

  createModalClose.addEventListener("click", () => {
    createGameModal.style.display = "none";
  });

  createGameSubmit.addEventListener("click", createGame);

  segmentButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const segment = parseInt(button.dataset.segment);
      selectSegment(segment);
    });
  });

  multiplierButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const multiplier = parseInt(button.dataset.multiplier);
      selectMultiplier(multiplier);
    });
  });

  submitTurnBtn.addEventListener("click", submitTurn);
  clearBtn.addEventListener("click", resetTurn);
  backBtn.addEventListener("click", removeLastThrow);

  modalClose.addEventListener("click", closeModal);
  modalOk.addEventListener("click", closeModal);
}

async function createGame() {
  const playerIds = playerIdsInput.value
    .split(",")
    .map((id) => parseInt(id.trim()))
    .filter((id) => !isNaN(id));
  const startingScore = parseInt(startingScoreSelect.value);
  const setsToWin = parseInt(setsToWinInput.value);
  const legsPerSet = parseInt(legsPerSetInput.value);
  const mustFinishOnDouble = mustFinishOnDoubleCheckbox.checked;

  if (playerIds.length === 0) {
    showMessage("Please enter at least one player ID", "error");
    return;
  }

  const createGameData = {
    type: 1,
    playerIds: playerIds,
    startingScore: startingScore,
    settings: {
      setsToWin: setsToWin,
      legsPerSet: legsPerSet,
      mustFinishOnDouble: mustFinishOnDouble,
    },
  };

  try {
    const response = await fetch(`${uiState.apiBaseUrl}/Games`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(createGameData),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || "Failed to create game");
    }

    const data = await response.json();
    uiState.gameId = data.id;
    uiState.startingScore = data.startingScore;
    uiState.players = data.players.map((player) => ({
      id: player.player.id,
      name: player.player.name,
      startingScore: data.startingScore,
      pointsThisTurn: 0,
      remainingScore: data.startingScore,
    }));
    uiState.activePlayerIndex = 0;

    renderPlayers();
    showMessage("Game created successfully", "success");
    createGameModal.style.display = "none";
  } catch (error) {
    showMessage(`Error: ${error.message}`, "error");
  }
}

function renderPlayers() {
  playersContainer.innerHTML = "";
  uiState.players.forEach((player, index) => {
    const playerCard = document.createElement("div");
    playerCard.className = `player-card ${
      index === uiState.activePlayerIndex ? "active" : ""
    }`;
    playerCard.innerHTML = `
      <div class="player-name">${player.name}</div>
      <div class="player-score">
        Start: ${player.startingScore} | This Turn: ${player.pointsThisTurn} | Remaining: ${player.remainingScore}
      </div>
    `;
    playersContainer.appendChild(playerCard);
  });
}

function selectSegment(segment) {
  uiState.selectedSegment = segment;
  segmentButtons.forEach((button) => {
    button.classList.toggle(
      "selected",
      parseInt(button.dataset.segment) === segment
    );
  });

  if (uiState.selectedMultiplier === null) {
    selectMultiplier(MULTIPLIERS.SINGLE);
  }

  checkIfCanAddThrow();
}

function selectMultiplier(multiplier) {
  uiState.selectedMultiplier = multiplier;
  multiplierButtons.forEach((button) => {
    button.classList.toggle(
      "selected",
      parseInt(button.dataset.multiplier) === multiplier
    );
  });
  checkIfCanAddThrow();
}

function checkIfCanAddThrow() {
  if (uiState.selectedSegment !== null && uiState.selectedMultiplier !== null) {
    if (
      uiState.selectedSegment === SEGMENTS.MISS &&
      uiState.selectedMultiplier !== MULTIPLIERS.SINGLE
    ) {
      showMessage("Miss can only be single (no multiplier)", "error");
      resetMultiplierSelection();
      return;
    }

    if (
      uiState.selectedSegment === SEGMENTS.BULL &&
      uiState.selectedMultiplier === MULTIPLIERS.TRIPLE
    ) {
      showMessage("Bull can only be single or double, not triple", "error");
      resetMultiplierSelection();
      return;
    }

    addThrow();
  }
}

function resetMultiplierSelection() {
  uiState.selectedMultiplier = null;
  multiplierButtons.forEach((button) => button.classList.remove("selected"));
}

function addThrow() {
  if (uiState.currentThrows.length >= 3) {
    showMessage("Maximum 3 throws per turn", "error");
    return;
  }

  const throwData = {
    Segment: uiState.selectedSegment,
    Multiplier: uiState.selectedMultiplier,
  };

  uiState.currentThrows.push(throwData);
  renderThrows();

  uiState.selectedSegment = null;
  uiState.selectedMultiplier = null;
  segmentButtons.forEach((button) => button.classList.remove("selected"));
  multiplierButtons.forEach((button) => button.classList.remove("selected"));

  submitTurnBtn.disabled = uiState.currentThrows.length === 0;
}

function renderThrows() {
  throwsList.innerHTML = "";
  uiState.currentThrows.forEach((throwData, index) => {
    const points = throwData.Segment * throwData.Multiplier;

    let throwText = "";

    if (throwData.Segment === SEGMENTS.MISS) {
      throwText = "Miss";
    } else if (throwData.Segment === SEGMENTS.BULL) {
      throwText =
        throwData.Multiplier === MULTIPLIERS.SINGLE
          ? "Single Bull"
          : "Double Bull";
    } else {
      const multiplierText =
        throwData.Multiplier === MULTIPLIERS.SINGLE
          ? "Single "
          : throwData.Multiplier === MULTIPLIERS.DOUBLE
          ? "Double "
          : "Triple ";
      throwText = `${multiplierText}${throwData.Segment}`;
    }

    const throwRow = document.createElement("div");
    throwRow.className = "throw-row";
    throwRow.innerHTML = `
      <div class="throw-info">
        <span class="throw-label">Throw ${index + 1}:</span>
        <span class="throw-value">${throwText}</span>
      </div>
      <div class="throw-points">+${points}</div>
    `;
    throwsList.appendChild(throwRow);
  });
}

function resetTurn() {
  uiState.currentThrows = [];
  uiState.selectedSegment = null;
  uiState.selectedMultiplier = null;
  segmentButtons.forEach((button) => button.classList.remove("selected"));
  multiplierButtons.forEach((button) => button.classList.remove("selected"));
  renderThrows();
  submitTurnBtn.disabled = true;
  clearMessage();
}

function removeLastThrow() {
  if (uiState.currentThrows.length === 0) {
    showMessage("No throws to remove", "error");
    return;
  }

  uiState.currentThrows.pop();
  renderThrows();
  submitTurnBtn.disabled = uiState.currentThrows.length === 0;
}

function submitTurn() {
  if (uiState.currentThrows.length === 0) {
    showMessage("Add at least one throw", "error");
    return;
  }
  if (!uiState.players.length) {
    showMessage("Create a game first", "error");
    return;
  }
  if (!uiState.gameId) {
    showMessage("Please create a game first", "error");
    return;
  }
  submitTurnToAPI();
}

async function submitTurnToAPI() {
  submitTurnBtn.disabled = true;
  const pointsThisTurn = uiState.currentThrows.reduce(
    (sum, throwData) => sum + throwData.Segment * throwData.Multiplier,
    0
  );

  try {
    const url = uiState.gameId
      ? `${uiState.apiBaseUrl}/GameRules/throws?gameId=${uiState.gameId}`
      : `${uiState.apiBaseUrl}/GameRules/throws`;
    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(uiState.currentThrows),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || "Failed to submit turn");
    }

    const data = await response.json();
    console.log(data);

    const currentPlayer = uiState.players[uiState.activePlayerIndex];
    if (currentPlayer) {
      currentPlayer.startingScore = currentPlayer.remainingScore;
      currentPlayer.pointsThisTurn = pointsThisTurn;
      currentPlayer.remainingScore = data.remainingScore;
    }

    if (data.message) {
      messageContainer.innerHTML = "";

      setTimeout(() => {
        showMessage(data.message, data.isBusted ? "error" : "success");
      }, 100);

      console.log("Displaying message:", data.message);
    } else {
      showMessage("Turn submitted successfully", "success");
    }

    if (data.gameComplete) {
      showModal("Game Complete", `Winner: ${getWinnerName()}`);
    } else if (data.setComplete) {
      showModal("Set Complete", `Winner: ${getWinnerName()}`);
    } else if (data.legComplete) {
      showModal("Leg Complete", `Winner: ${getWinnerName()}`);
    }

    uiState.activePlayerIndex =
      (uiState.activePlayerIndex + 1) % uiState.players.length;

    renderPlayers();
    resetTurn();
  } catch (error) {
    showMessage(`Error: ${error.message}`, "error");
    submitTurnBtn.disabled = false;
  }
}

function showMessage(message, type = "info") {
  messageContainer.innerHTML = `<div class="message ${type}">${message}</div>`;
}

function clearMessage() {
  messageContainer.innerHTML = "";
}

function showModal(title, message) {
  modalTitle.textContent = title;
  modalMessage.textContent = message;
  winnerModal.style.display = "block";
}

function closeModal() {
  winnerModal.style.display = "none";
}

function getWinnerName() {
  const currentPlayer = uiState.players[uiState.activePlayerIndex];
  return currentPlayer ? currentPlayer.name : "Unknown";
}

init();
