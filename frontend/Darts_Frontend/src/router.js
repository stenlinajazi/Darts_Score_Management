import NavBar from "./components/nav-bar/NavBar.js";
import GamesListWrapper from "./components/game/GamesListWrapper.js";
import PlayGame from "./components/play-game/PlayGame.js";
import Players from "./components/players/Players.js";
const Router = (function () {
  const basePath = "/Darts_Frontend";
  const routes = [
    {
      path: "/",
      data: GamesListWrapper,
    },
    {
      path: "/play-game",
      data: PlayGame,
    },
    {
      path: "/players",
      data: Players,
    },
  ];

  let root;

  function router(path) {
    let adjustedPath = path;
    if (basePath && path.startsWith(basePath)) {
      adjustedPath = path.substring(basePath.length) || "/";
    }

    const route = routes.find((route) => route.path === adjustedPath);

    if (route) {
      root.innerHTML = "";
      NavBar(root);
      route.data(root);
    } else {
      root.innerHTML = "<h1>Page Not Found</h1>";
    }
  }

  function handlePopstate() {
    const path = window.location.pathname;
    router(path);
  }

  return {
    init: function (appRoot) {
      root = appRoot;
      window.addEventListener("DOMContentLoaded", function () {
        NavBar(root);
        handlePopstate();
      });

      window.addEventListener("popstate", handlePopstate);
    },
    router,
  };
})();

export default Router;
