import {
  formatThrowText,
  calculateThrowPoints,
} from "../../services/gameService.js";

const ThrowsList = (throws) => {
  const list = document.createElement("div");
  list.id = "throws-list";

  throws.forEach((throwData, index) => {
    const normalizedThrow = {
      segment: throwData.segment || throwData.Segment || 0,
      multiplier: throwData.multiplier || throwData.Multiplier || 0,
    };
    const points = calculateThrowPoints(throwData);
    const throwText = formatThrowText(throwData);

    const throwRow = document.createElement("div");
    throwRow.className = "throw-row";
    throwRow.innerHTML = `
      <div class="throw-info">
        <span class="throw-label">Throw ${index + 1}:</span>
        <span class="throw-value">${throwText}</span>
      </div>
      <div class="throw-points">+${points}</div>
    `;
    list.appendChild(throwRow);
  });

  return list;
};

export default ThrowsList;
