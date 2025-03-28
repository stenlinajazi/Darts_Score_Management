const PlayerCard = (player, isActive, gameState) => {
  const card = document.createElement("div");
  card.className = `player-card ${isActive ? "active" : ""}`;

  let legsWonInCurrentSet = 0;
  for (let leg = 1; leg <= gameState.legsPerSet; leg++) {
    const legKey = leg.toString();
    if (
      gameState.legScores[legKey] &&
      gameState.legScores[legKey][player.id] > 0
    ) {
      legsWonInCurrentSet += gameState.legScores[legKey][player.id];
    }
  }

  card.innerHTML = `
    <div class="player-name">${player.name}</div>
    <div class="player-score">
      Start: ${player.startingScore} | This Turn: ${
    player.pointsThisTurn
  } | Remaining: ${player.remainingScore}
    </div>
    <div class="player-stats">
      Sets Won: ${gameState.setScores[player.id] || 0} | Legs Won in Set ${
    gameState.currentSetNumber
  }: ${legsWonInCurrentSet}
    </div>
  `;
  return card;
};

export default PlayerCard;
