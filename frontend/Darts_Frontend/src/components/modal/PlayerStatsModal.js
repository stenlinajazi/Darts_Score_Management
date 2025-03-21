import { getPlayerStats } from "../../services/apiService.js";

const PlayerStatsModal = async (playerId) => {
  const modal = document.createElement("div");
  modal.id = "player-stats-modal";
  modal.className = "modal";
  modal.style.display = "none";
  modal.innerHTML = `
    <div class="modal-content">
      <span id="player-stats-modal-close" class="modal-close">×</span>
      <h2>Player Stats</h2>
      <div id="player-stats-content">
      </div>
      <div id="player-stats-actions" style="margin-top: 20px; display: none;">
        <button id="player-stats-close" class="btn btn-secondary">Close</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);

  const statsContent = document.getElementById("player-stats-content");
  const actionsContainer = document.getElementById("player-stats-actions");

  const fetchStats = async () => {
    actionsContainer.style.display = "none";

    try {
      const stats = await getPlayerStats(playerId);
      if (!stats) {
        throw new Error("Invalid stats data");
      }

      if (!stats.last10LegsStats || !stats.allStats) {
        throw new Error("Incomplete stats data structure");
      }

      statsContent.innerHTML = `
        <p><strong>Player:</strong> ${stats.playerName} (ID: ${stats.playerId})</p>
        <p><strong>Total Legs Played:</strong> ${stats.totalLegsPlayed}</p>
        <p><strong>Legs Won:</strong> ${stats.legsWon}</p>
        <h3>Last 10 Legs Stats</h3>
        <table>
          <tr><th>Metric</th><th>Average</th><th>Best</th></tr>
          <tr><td>PPD</td><td>${stats.last10LegsStats.ppd.average}</td><td>${stats.last10LegsStats.ppd.best}</td></tr>
          <tr><td>First 9 PPD</td><td>${stats.last10LegsStats.first9PPD.average}</td><td>${stats.last10LegsStats.first9PPD.best}</td></tr>
          <tr><td>Checkout %</td><td>${stats.last10LegsStats.checkoutPercentage.average}</td><td>${stats.last10LegsStats.checkoutPercentage.best}</td></tr>
          <tr><td>Win %</td><td>${stats.last10LegsStats.winPercentage.average}</td><td>${stats.last10LegsStats.winPercentage.best}</td></tr>
        </table>
        <h3>All-Time Stats</h3>
        <table>
          <tr><th>Metric</th><th>Total</th><th>Per Leg</th></tr>
          <tr><td>60+ Scores</td><td>${stats.allStats.count60Plus.total}</td><td>${stats.allStats.count60Plus.perLeg}</td></tr>
          <tr><td>100+ Scores</td><td>${stats.allStats.count100Plus.total}</td><td>${stats.allStats.count100Plus.perLeg}</td></tr>
          <tr><td>140+ Scores</td><td>${stats.allStats.count140Plus.total}</td><td>${stats.allStats.count140Plus.perLeg}</td></tr>
          <tr><td>180s</td><td>${stats.allStats.count180s.total}</td><td>${stats.allStats.count180s.perLeg}</td></tr>
        </table>
      `;
    } catch (error) {
      console.error("Error fetching player stats:", error);
      statsContent.innerHTML = `<p style="color: red;">${error.message}</p>`;
      actionsContainer.style.display = "block";
    }
  };

  fetchStats();

  const closeModal = () => {
    modal.style.display = "none";
    modal.remove();
  };

  document
    .getElementById("player-stats-modal-close")
    .addEventListener("click", closeModal);
  document
    .getElementById("player-stats-close")
    .addEventListener("click", closeModal);

  modal.style.display = "block";
};

export default PlayerStatsModal;
