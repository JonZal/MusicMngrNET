import jwt from "jwt-decode";
import { handleResponse } from "../helpers/handleResponse";
import axios from "axios";
import { authenticationService } from "./authentication";

export const playlistService = {
    getAllPlaylists: getAllPlaylists,
    getPlaylist: getPlaylist,
    postPlaylist: postPlaylist,
    putPlaylist: putPlaylist,
    deletePlaylist: deletePlaylist
};

async function getAllPlaylists() {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);
    let response = await axios.get(`https://localhost:44384/api/users/${userInfo.id}/playlists`, {
        headers: {
            Authorization: `Bearer ${currentUser.token}`
        }
    });
    return handleResponse(response);
}
async function getPlaylist(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);
    let response = await axios.get(
        `https://localhost:44384/api/users/${userInfo.id}/playlists/${id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
async function postPlaylist(playlist) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);
    let response = await axios.post(
        `https://localhost:44384/api/users/${userInfo.id}/playlists`,
        playlist,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
async function putPlaylist(playlist) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);
    let response = await axios.put(
        `https://localhost:44384/api/users/${userInfo.id}/playlists/${playlist.id}`,
        playlist,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
async function deletePlaylist(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    const userInfo = jwt(currentUser.token);
    let response = await axios.delete(
        `https://localhost:44384/api/users/${userInfo.id}/playlists/${id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
