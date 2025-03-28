import CreateGameModal from "../modal/CreateGameModal.js";
import WinnerModal from "../modal/WinnerModal.js";
import PlayerCard from "./PlayerCard.js";
import ThrowsList from "./ThrowsList.js";
import { submitThrows, fetchGameState } from "../../services/apiService.js";
import { MULTIPLIERS } from "../../services/gameService.js";

const PlayGame = (root) => {
  let state = {
    gameId: null,
    startingScore: null,
    players: [],
    activePlayerIndex: 0,
    selectedSegment: null,
    selectedMultiplier: null,
    currentThrows: [],
  };

  const wrapper = document.createElement("div");
  wrapper.id = "game-play-container";
  wrapper.className = "view";
  root.appendChild(wrapper);

  const render = () => {
    if (!state.gameId) {
      wrapper.innerHTML = `
        <div class="game-info">
          <h2>Play Game</h2>
          <p>Please create a game to start playing.</p>
        </div>
      `;
      openCreateGameModal();
      return;
    }

    wrapper.innerHTML = `
    <div id="players-container" class="players-container"></div>
    <div id="message-container"></div>
    <div class="throws-container">
      <h3>Current Throws</h3>
      <div id="throws-list"></div>
    </div>
    <h3>Select Segment</h3>
    <div class="score-grid">
      <button class="segment-btn miss" data-segment="0">Miss</button>
      <button class="segment-btn" data-segment="1">1</button>
      <button class="segment-btn" data-segment="2">2</button>
      <button class="segment-btn" data-segment="3">3</button>
      <button class="segment-btn" data-segment="4">4</button>
      <button class="segment-btn" data-segment="5">5</button>
      <button class="segment-btn" data-segment="6">6</button>
      <button class="segment-btn" data-segment="7">7</button>
      <button class="segment-btn" data-segment="8">8</button>
      <button class="segment-btn" data-segment="9">9</button>
      <button class="segment-btn" data-segment="10">10</button>
      <button class="segment-btn" data-segment="11">11</button>
      <button class="segment-btn" data-segment="12">12</button>
      <button class="segment-btn" data-segment="13">13</button>
      <button class="segment-btn" data-segment="14">14</button>
      <button class="segment-btn" data-segment="15">15</button>
      <button class="segment-btn" data-segment="16">16</button>
      <button class="segment-btn" data-segment="17">17</button>
      <button class="segment-btn" data-segment="18">18</button>
      <button class="segment-btn" data-segment="19">19</button>
      <button class="segment-btn" data-segment="20">20</button>
      <button class="segment-btn bull" data-segment="25">Bull</button>
    </div>
    <h3>Select Multiplier</h3>
    <div class="multiplier-grid">
      <button class="multiplier-btn" data-multiplier="1">Single</button>
      <button class="multiplier-btn" data-multiplier="2">Double</button>
      <button class="multiplier-btn" data-multiplier="3">Triple</button>
    </div>
    <div class="actions">
      <button id="back-btn" class="btn btn-secondary">Back</button>
      <button id="clear-btn" class="btn btn-secondary">Clear</button>
      <button id="submit-turn-btn" class="btn btn-primary" disabled>Submit Turn</button>
    </div>
  `;
    attachEventListeners();
    renderPlayers();
    renderThrows();

    state.selectedMultiplier = MULTIPLIERS[0];
  };

  const openCreateGameModal = () => {
    CreateGameModal((newGame) => {
      initializeGameState(newGame);
      render();
      showMessage("Game created successfully", "success");
    });
  };

  const initializeGameState = (newGame) => {
    state.gameId = newGame.id;
    state.startingScore = newGame.startingScore;
    state.players = newGame.players.map((player) => ({
      id: player.player.id,
      name: player.player.name,
      startingScore: newGame.startingScore,
      pointsThisTurn: 0,
      remainingScore: newGame.startingScore,
      setsWon: 0,
      legsWon: 0,
    }));
    state.activePlayerIndex = 0;
    state.currentThrows = [];
    state.selectedSegment = null;
    state.selectedMultiplier = null;
  };

  const resumeGameState = async (gameId) => {
    try {
      const gameState = await fetchGameState(gameId);
      state.gameId = gameState.gameId;
      state.startingScore = gameState.startingScore;
      state.players = gameState.players.map((player) => ({
        id: player.id,
        name: player.name,
        startingScore: player.startingScore,
        pointsThisTurn: player.pointsThisTurn,
        remainingScore: player.remainingScore,
        setsWon: gameState.setScores[player.id] || 0,
        legsWon:
          gameState.legScores[gameState.currentLegNumber]?.[player.id] || 0,
      }));
      state.activePlayerIndex = gameState.activePlayerIndex;
      state.currentThrows = gameState.currentThrows || [];
      state.selectedSegment = null;
      state.selectedMultiplier = null;
      render();
      showMessage(gameState.message || "Game resumed successfully", "success");
    } catch (error) {
      showMessage(`Failed to resume game: ${error.message}`, "error");
      state.gameId = null;
      render();
    }
  };

  const resetLegState = () => {
    state.currentThrows = [];
    state.selectedSegment = null;
    state.selectedMultiplier = null;
    state.players.forEach((player) => {
      player.remainingScore = state.startingScore;
      player.pointsThisTurn = 0;
      player.startingScore = state.startingScore;
    });
  };

  const renderPlayers = () => {
    const playersContainer = document.getElementById("players-container");
    if (!playersContainer) return;
    playersContainer.innerHTML = "";
    state.players.forEach((player, index) => {
      playersContainer.appendChild(
        PlayerCard(player, index === state.activePlayerIndex)
      );
    });
  };

  const renderThrows = () => {
    const throwsListContainer = document.getElementById("throws-list");
    if (!throwsListContainer) return;
    throwsListContainer.innerHTML = "";
    throwsListContainer.appendChild(ThrowsList(state.currentThrows));
  };

  const showMessage = (message, type = "info") => {
    const messageContainer = document.getElementById("message-container");
    if (!messageContainer) return;
    messageContainer.innerHTML = `<div class="message ${type}">${message}</div>`;
  };

  const clearMessage = () => {
    const messageContainer = document.getElementById("message-container");
    if (!messageContainer) return;
    messageContainer.innerHTML = "";
  };

  const selectSegment = (segment) => {
    state.selectedSegment = segment;
    document.querySelectorAll(".segment-btn").forEach((button) => {
      button.classList.toggle(
        "selected",
        parseInt(button.dataset.segment) === segment
      );
    });

    if (state.selectedMultiplier === null) {
      selectMultiplier(MULTIPLIERS[0]);
    }

    validateAndAddThrow();
  };

  const selectMultiplier = (multiplier) => {
    state.selectedMultiplier = multiplier;
    document.querySelectorAll(".multiplier-btn").forEach((button) => {
      button.classList.toggle(
        "selected",
        parseInt(button.dataset.multiplier) === multiplier
      );
    });
    if (state.selectedSegment !== null) {
      validateAndAddThrow();
    }
  };

  const validateAndAddThrow = () => {
    if (state.selectedSegment === null || state.selectedMultiplier === null) {
      return;
    }

    if (state.selectedSegment === 0) {
      if (state.selectedMultiplier !== MULTIPLIERS[0]) {
        showMessage("Miss can only be single (no multiplier)", "error");
        selectMultiplier(MULTIPLIERS[0]);
        return;
      }
    }

    if (state.selectedSegment === 25) {
      if (state.selectedMultiplier === 3) {
        showMessage("Bull can only be single or double, not triple", "error");
        state.selectedSegment = null;
        document
          .querySelectorAll(".segment-btn")
          .forEach((button) => button.classList.remove("selected"));
        return;
      }
    }

    addThrow();
  };

  const addThrow = () => {
    if (state.currentThrows.length >= 3) {
      showMessage("Maximum 3 throws per turn", "error");
      return;
    }

    const throwData = {
      segment: state.selectedSegment,
      multiplier: state.selectedMultiplier,
    };

    state.currentThrows.push(throwData);
    renderThrows();

    state.selectedSegment = null;

    document
      .querySelectorAll(".segment-btn")
      .forEach((button) => button.classList.remove("selected"));
    document.getElementById("submit-turn-btn").disabled =
      state.currentThrows.length === 0;

    clearMessage();
  };

  const resetTurn = () => {
    state.currentThrows = [];
    state.selectedSegment = null;

    document
      .querySelectorAll(".segment-btn")
      .forEach((button) => button.classList.remove("selected"));

    renderThrows();
    document.getElementById("submit-turn-btn").disabled = true;
    clearMessage();
  };

  const removeLastThrow = () => {
    if (state.currentThrows.length === 0) {
      showMessage("No throws to remove", "error");
      return;
    }

    state.currentThrows.pop();
    renderThrows();
    document.getElementById("submit-turn-btn").disabled =
      state.currentThrows.length === 0;
  };

  const submitTurn = async () => {
    if (state.currentThrows.length === 0) {
      showMessage("Add at least one throw", "error");
      return;
    }
    if (!state.players.length || !state.gameId) {
      showMessage("Please create a game first", "error");
      return;
    }

    const pointsThisTurn = state.currentThrows.reduce(
      (sum, throwData) => sum + throwData.segment * throwData.multiplier,
      0
    );

    try {
      const data = await submitThrows(state.gameId, state.currentThrows);
      const currentPlayer = state.players[state.activePlayerIndex];

      if (currentPlayer) {
        currentPlayer.startingScore = currentPlayer.remainingScore;
        currentPlayer.pointsThisTurn = pointsThisTurn;
        currentPlayer.remainingScore = data.remainingScore;
      }

      if (data.message) {
        clearMessage();
        setTimeout(() => {
          showMessage(data.message, data.isBusted ? "error" : "success");
        }, 100);
      } else {
        showMessage("Turn submitted successfully", "success");
      }

      if (data.gameComplete) {
        WinnerModal("Game Complete", `Winner: ${currentPlayer.name}`, () => {
          state.gameId = null;
          state.players = [];
          render();
        });
      } else if (data.setComplete) {
        currentPlayer.setsWon = (currentPlayer.setsWon || 0) + 1;
        WinnerModal("Set Complete", `Winner: ${currentPlayer.name}`, () => {
          resetLegState();
          state.activePlayerIndex = 0;
          renderPlayers();
          renderThrows();
          showMessage("New set started", "info");
        });
      } else if (data.legComplete) {
        currentPlayer.legsWon = (currentPlayer.legsWon || 0) + 1;
        WinnerModal("Leg Complete", `Winner: ${currentPlayer.name}`, () => {
          resetLegState();
          state.activePlayerIndex = 0;
          renderPlayers();
          renderThrows();
          showMessage("New leg started", "info");
        });
      } else {
        state.activePlayerIndex =
          (state.activePlayerIndex + 1) % state.players.length;
        renderPlayers();
        resetTurn();
      }
    } catch (error) {
      showMessage(`Error: ${error.message}`, "error");
      document.getElementById("submit-turn-btn").disabled = false;
    }
  };

  const attachEventListeners = () => {
    document.querySelectorAll(".segment-btn").forEach((button) => {
      button.addEventListener("click", () => {
        const segment = parseInt(button.dataset.segment);
        selectSegment(segment);
      });
    });

    document.querySelectorAll(".multiplier-btn").forEach((button) => {
      button.addEventListener("click", () => {
        const multiplier = parseInt(button.dataset.multiplier);
        selectMultiplier(multiplier);
      });
    });

    document
      .getElementById("submit-turn-btn")
      .addEventListener("click", submitTurn);
    document.getElementById("clear-btn").addEventListener("click", resetTurn);
    document
      .getElementById("back-btn")
      .addEventListener("click", removeLastThrow);
  };

  const historyState = window.history.state;
  if (historyState && historyState.gameId) {
    resumeGameState(historyState.gameId);
  } else {
    render();
  }
};

export default PlayGame;
