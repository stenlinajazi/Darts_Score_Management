const NavBar = (root) => {
  const basePath = "/Darts_Frontend";
  const nav = document.createElement("div");
  nav.className = "navigation";
  nav.innerHTML = `
    <button id="games-list-btn" class="btn btn-secondary">Games List</button>
    <button id="play-game-btn" class="btn btn-secondary">Play Game</button>
    <button id="players-btn" class="btn btn-secondary">Players</button>
  `;

  root.appendChild(nav);

  document.getElementById("games-list-btn").addEventListener("click", () => {
    window.history.pushState({}, "", `${basePath}/`);
    window.dispatchEvent(new PopStateEvent("popstate"));
  });

  document.getElementById("play-game-btn").addEventListener("click", () => {
    window.history.pushState({}, "", `${basePath}/play-game`);
    window.dispatchEvent(new PopStateEvent("popstate"));
  });
  document.getElementById("players-btn").addEventListener("click", () => {
    window.history.pushState({}, "", `${basePath}/players`);
    window.dispatchEvent(new PopStateEvent("popstate"));
  });
};

export default NavBar;
