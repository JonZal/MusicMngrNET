import React from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";
import UsersTable from "./components/usersTable";
import Home from "./components/home";
import Playlists from "./components/playlists";
import Playlist from "./components/playlist";
import MyPlaylists from "./components/myPlaylists";
import MySongs from "./components/mySongs";
import { LoginPage } from "./pages/login";
import Register from "./pages/register";
//import { HomePage } from "./pages/register";
import { history } from "./helpers/history";
import Navbarr from "./components/navBar";
import Footer from "./components/Footer";

function App() {
  return (
    <Router history={history}>
      <div>
        <Navbarr history={history} />
        <Switch>
          <Route exact path="/" component={Home}></Route>
          <Route exact path="/users">
            <UsersTable />;
          </Route>
          <Route
            exact
            path="/login"
            render={(props) => <LoginPage {...props} />}
          />
          <Route
            exact
            path="/register"
            render={(props) => <Register {...props} />}
          />
          <Route exact path="/playlists">
            <Playlists />
          </Route>
          <Route
            exact
            path="/playlists/:id"
            render={(props) => <Playlist {...props} />}
          />
          <Route path="/users/:id/playlists">
            <MyPlaylists />
          </Route>
          {/* <Route path="/users/:id/songs">
                        <MySongs />
                    </Route> */}
          <Route
            exact
            path="/users/:id/songs"
            render={(props) => <MySongs {...props} />}
          />
        </Switch>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
