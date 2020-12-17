import React, { Component } from "react";
import { Link } from "react-router-dom";
import { playlistService } from "../services/playlistService";
import { authenticationService } from "../services/authentication";
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

class Playlists extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            isLoaded: false,
            newPlaylist: {
                Title: "",
                Description: "",
                User: {
                    UserId: 0
                }
            },
            editPlaylist: {
                id: "",
                Title: "",
                Description: "",
                User: {
                    UserId: 0
                }
            },
            newPlaylistModal: false,
            editModalModal: false,
            user: jwt(authenticationService.currentUserValue.token)
        };
    }

    handleNewPlaylistChange = event => {
        event.preventDefault();
        const name = event.target.name;
        const { newPlaylist } = this.state;
        newPlaylist[name] = event.target.value;
        this.setState({ newPlaylist });
    };

    handleEditPlaylistChange = event => {
        event.preventDefault();
        const name = event.target.name;
        const { editPlaylist } = this.state;
        editPlaylist[name] = event.target.value;
        this.setState({ editPlaylist });
    };

    toggleNewPlaylistModal = () => {
        this.setState({
            newPlaylistModal: !this.state.newPlaylistModal
        });
    };
    toggleEditPlaylistModal = () => {
        this.setState({
            editModalModal: !this.state.editModalModal
        });
    };
    addPlaylist = () => {
        let { newPlaylist } = this.state;
        const user = jwt(authenticationService.currentUserValue.token);
        newPlaylist.User.UserId = user.id;
        playlistService.postPlaylist(newPlaylist).then(playlist => {
            let { data } = this.state;
            data.push(playlist);
            this.setState({
                data,
                newPlaylistModal: false,
                newPlaylist: {
                    Title: "",
                    Description: "",
                    User: {
                        UserId: 0
                    }
                }
            });
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
        const user = jwt(authenticationService.currentUserValue.token);
        this.setState({
            editModalModal: !this.state.editModalModal,
            editPlaylist: {
                id: playlist.id,
                Title: playlist.title,
                Description: playlist.description,
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
        playlistService
            .getAllPlaylists()
            .then(data => this.setState({ data: data, isLoaded: true }));
    };

    render() {
        const { data, isLoaded, user } = this.state;
        return (
            <div>
                <div className="App container">
                    <h1>Playlists page</h1>

                    <Button
                        className="my-3"
                        color="primary"
                        onClick={this.toggleNewPlaylistModal}
                    >
                        Add playlist
                    </Button>

                    <Modal
                        isOpen={this.state.newPlaylistModal}
                        toggle={this.toggleNewPlaylistModal}
                    >
                        <ModalHeader toggle={this.toggleNewPlaylistModal}>
                            Add a new playlist
                        </ModalHeader>
                        <ModalBody>
                            <FormGroup>
                                <Label for="title">Title</Label>
                                <Input
                                    id="title"
                                    name="Title"
                                    value={this.state.newPlaylist.Title}
                                    onChange={this.handleNewPlaylistChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Label for="rating">Description</Label>
                                <Input
                                    id="text"
                                    name="Description"
                                    type="textarea"
                                    value={this.state.newPlaylist.Description}
                                    onChange={this.handleNewPlaylistChange}
                                />
                            </FormGroup>
                        </ModalBody>
                        <ModalFooter>
                            <Button color="primary" onClick={this.addPlaylist}>
                                Add Playlist
                            </Button>{" "}
                            <Button
                                color="secondary"
                                onClick={this.toggleNewPlaylistModal}
                            >
                                Cancel
                            </Button>
                        </ModalFooter>
                    </Modal>

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
                            <Button
                                color="primary"
                                onClick={this.updatePlaylist}
                            >
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

                    <Table>
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
                                        <td>
                                            <Link
                                                className="nav-link"
                                                to={"/playlists/" + playlist.id}
                                            >
                                                {playlist.title}
                                            </Link>
                                        </td>
                                        <td>{playlist.text}</td>
                                        <td>
                                            {user.id == playlist.userId ? (
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
                                            ) : (
                                                ""
                                            )}
                                            {user.role === "Admin" ||
                                            user.id == playlist.userId ? (
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
                                            ) : (
                                                ""
                                            )}
                                        </td>
                                    </tr>
                                ))
                            ) : (
                                <tr></tr>
                            )}
                        </tbody>
                    </Table>
                </div>
            </div>
        );
    }
}

export default Playlists;
