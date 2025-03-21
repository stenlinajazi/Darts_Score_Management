export const SEGMENTS = Array.from({ length: 20 }, (_, i) => i + 1).concat([
  25,
]);
export const MULTIPLIERS = [1, 2, 3];

export const formatThrowText = (throwData) => {
  const segment = throwData.segment;
  const multiplier = throwData.multiplier;

  if (segment === 0 && multiplier === 1) {
    return "Miss";
  }

  let multiplierText;
  switch (multiplier) {
    case 1:
      multiplierText = "Single";
      break;
    case 2:
      multiplierText = "Double";
      break;
    case 3:
      multiplierText = "Triple";
      break;
    default:
      multiplierText = "Unknown";
  }

  const segmentText = segment === 25 ? "Bullseye" : segment;

  return `${multiplierText} ${segmentText}`;
};

export const calculateThrowPoints = (throwData) => {
  return throwData.segment * throwData.multiplier;
};
