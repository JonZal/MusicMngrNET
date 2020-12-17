import jwt from "jwt-decode";
import { handleResponse } from "../helpers/handleResponse";
import axios from "axios";
import { authenticationService } from "./authentication";

export const userService = {
    getAllUsers: getAllUsers,
    getUser: getUser,
    getCurrentUser: getCurrentUser,
    getCurrentUserPlaylists: getCurrentUserPlaylists,
    getCurrentUserPlaylist: getCurrentUserPlaylist,
    getCurrentUserSongs: getCurrentUserSongs,
    getCurrentUserSong: getCurrentUserSong,
    postUser: postUser,
    putUser: putUser,
    deleteUser: deleteUser
};

async function getAllUsers() {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.get(`https://localhost:44384/api/users`, {
        headers: {
            Authorization: `Bearer ${currentUser.token}`
        }
    });
    return handleResponse(response);
}

async function getUser(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.get(`https://localhost:44384/api/users/${id}`, {
        headers: {
            Authorization: `Bearer ${currentUser.token}`
        }
    });
    return handleResponse(response);
}

async function getCurrentUser() {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);

    let response = await axios.get(
        `https://localhost:44384/api/users/${userInfo.id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}

async function getCurrentUserPlaylists() {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);

    let response = await axios.get(
        `https://localhost:44384/api/users/${userInfo.id}/Playlists`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}

async function getCurrentUserPlaylist(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);

    let response = await axios.get(
        `https://localhost:44384/api/users/${userInfo.id}/Playlists/${id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}

async function getCurrentUserSongs(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);

    let response = await axios.get(
        `https://localhost:44384/api/users/${userInfo.id}/Playlists/${id}/Songs`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}

async function getCurrentUserSong(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);

    let response = await axios.get(
        `https://localhost:44384/api/users/${userInfo.id}/Songs/${id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}

async function postUser(user) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.post(`https://localhost:44384/api/users`, user, {
        headers: {
            Authorization: `Bearer ${currentUser.token}`
        }
    });
    return handleResponse(response);
}
async function putUser(user) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.put(
        `https://localhost:44384/api/users/${user.id}`,
        user,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
async function deleteUser(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.delete(
        `https://localhost:44384/api/users/${id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
