const PlayerCard = (player, isActive) => {
  const card = document.createElement("div");
  card.className = `player-card ${isActive ? "active" : ""}`;
  card.innerHTML = `
    <div class="player-name">${player.name}</div>
    <div class="player-score">
      Start: ${player.startingScore} | This Turn: ${player.pointsThisTurn} | Remaining: ${player.remainingScore}
    </div>
  `;
  return card;
};

export default PlayerCard;
