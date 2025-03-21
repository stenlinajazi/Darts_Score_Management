import GameCard from "./GameCard.js";
import { fetchGames } from "../../services/apiService.js";

const GamesList = async (onDetailsClick, onDeleteClick) => {
  try {
    const games = await fetchGames();
    return games.map((game) => GameCard(game, onDetailsClick, onDeleteClick));
  } catch (error) {
    console.error("Error fetching games:", error.message);
    return [];
  }
};

export default GamesList;
