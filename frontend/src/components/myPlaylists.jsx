import React, { Component } from "react";
import { authenticationService } from "../services/authentication";
import { playlistService } from "../services/playlistService";
import { userService } from "../services/userService";
import jwt from "jwt-decode";
import {
    Input,
    FormGroup,
    Label,
    Modal,
    ModalHeader,
    ModalBody,
    ModalFooter,
    Table,
    Button
} from "reactstrap";

class MyPlaylists extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            isLoaded: false,
            editPlaylist: {
                id: "",
                Title: "",
                Description: "",
                User: {
                    UserId: 0
                }
            },
            editModalModal: false,
            user: jwt(authenticationService.currentUserValue.token)
        };
    }
    handleEditPlaylistChange = event => {
        event.preventDefault();
        const name = event.target.name;
        const { editPlaylist } = this.state;
        editPlaylist[name] = event.target.value;
        this.setState({ editPlaylist });
    };
    toggleEditPlaylistModal = () => {
        this.setState({
            editModalModal: !this.state.editModalModal
        });
    };
    updatePlaylist = () => {
        const { editPlaylist } = this.state;
        playlistService.putPlaylist(editPlaylist).then(response => {
            this._refreshPlaylists();
            this.setState({
                editModalModal: !this.state.editModalModal
            });
        });
    };
    editPlaylist(playlist) {
        const { user } = this.state;
        this.setState({
            editModalModal: !this.state.editModalModal,
            editPlaylist: {
                id: playlist.id,
                Title: playlist.title,
                Description: playlist.text,
                User: {
                    UserId: user.id
                }
            }
        });
    }
    deletePlaylist(id) {
        playlistService.deletePlaylist(id).then(response => {
            this._refreshPlaylists();
        });
    }
    componentDidMount() {
        this._refreshPlaylists();
    }
    _refreshPlaylists = () => {
        userService
            .getCurrentUserPlaylists()
            .then(data => this.setState({ data: data, isLoaded: true }));
    };
    render() {
        const { isLoaded, data } = this.state;
        return (
            <div className="App container">
                <h1>My playlists page</h1>
                <Modal
                    isOpen={this.state.editModalModal}
                    toggle={this.toggleEditPlaylistModal}
                >
                    <ModalHeader toggle={this.toggleEditPlaylistModal}>
                        Edit playlist
                    </ModalHeader>
                    <ModalBody>
                        <FormGroup>
                            <Label for="title">Title</Label>
                            <Input
                                id="title"
                                name="Title"
                                value={this.state.editPlaylist.Title}
                                onChange={this.handleEditPlaylistChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="text">Description</Label>
                            <Input
                                id="text"
                                name="Description"
                                value={this.state.editPlaylist.Description}
                                onChange={this.handleEditPlaylistChange}
                            />
                        </FormGroup>
                    </ModalBody>
                    <ModalFooter>
                        <Button color="primary" onClick={this.updatePlaylist}>
                            Update Playlist
                        </Button>{" "}
                        <Button
                            color="secondary"
                            onClick={this.toggleEditPlaylistModal}
                        >
                            Cancel
                        </Button>
                    </ModalFooter>
                </Modal>

                <Table className="table">
                    <thead>
                        <tr>
                            <th>Title</th>
                            <th>Description</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoaded ? (
                            data.map(playlist => (
                                <tr key={playlist.id}>
                                    <td>{playlist.Title}</td>
                                    <td>{playlist.Description}</td>
                                    <td>
                                        <Button
                                            color="success"
                                            size="sm"
                                            className="mr-2"
                                            onClick={this.editPlaylist.bind(
                                                this,
                                                playlist
                                            )}
                                        >
                                            Edit
                                        </Button>
                                        <Button
                                            color="danger"
                                            size="sm"
                                            onClick={this.deletePlaylist.bind(
                                                this,
                                                playlist.id
                                            )}
                                        >
                                            Delete
                                        </Button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr></tr>
                        )}
                    </tbody>
                </Table>
            </div>
        );
    }
}

export default MyPlaylists;
