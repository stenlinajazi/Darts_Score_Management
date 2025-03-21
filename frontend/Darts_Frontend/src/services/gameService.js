export const SEGMENTS = Array.from({ length: 20 }, (_, i) => i + 1).concat([
  25,
]);
export const MULTIPLIERS = [1, 2, 3];

export const formatThrowText = (throwData) => {
  const segment = throwData.segment || throwData.Segment || 0;
  const multiplier = throwData.multiplier || throwData.Multiplier || 0;

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
  const segment = throwData.segment || throwData.Segment || 0;
  const multiplier = throwData.multiplier || throwData.Multiplier || 0;

  return segment * multiplier;
};
