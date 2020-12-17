import jwt from "jwt-decode";
import { handleResponse } from "../helpers/handleResponse";
import axios from "axios";
import { authenticationService } from "../services/authentication";

export const songService = {
    getAllSongs: getAllSongs,
    postSong: postSong,
    putSong: putSong,
    deleteSong: deleteSong
};

async function getAllSongs() {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.get(`https://localhost:44384/api/songs`, {
        headers: {
            Authorization: `Bearer ${currentUser.token}`
        }
    });
    return handleResponse(response);
}
async function postSong(song) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.post(
        `https://localhost:44384/api/songs`,
        song,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
async function putSong(song) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.put(
        `https://localhost:44384/api/songs/${song.id}`,
        song,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
async function deleteSong(id) {
    const currentUser = authenticationService.currentUserValue;
    if (authenticationService.refreshStatus) {
        authenticationService.refresh();
    }
    let response = await axios.delete(
        `https://localhost:44384/api/songs/${id}`,
        {
            headers: {
                Authorization: `Bearer ${currentUser.token}`
            }
        }
    );
    return handleResponse(response);
}
