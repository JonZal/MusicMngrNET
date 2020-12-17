import React, { Component } from "react";
import { playlistService } from "../services/playlistService";
import { songService } from "../services/songService";
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

class Playlist extends Component {
    constructor(props) {
        super(props);
        this.state = {
            playlistId: this.props.match.params.id,
            data: {},
            isLoaded: false,
            user: jwt(authenticationService.currentUserValue.token),
            newSongModal: false,
            editSongModal: false,
            newSong: {
                Text: "",
                Playlist: {
                    Id: this.props.match.params.id
                },
                User: {
                    UserId: ""
                }
            },
            editSong: {}
        };
    }

    handleNewSongChange = event => {
        event.preventDefault();
        const name = event.target.name;
        const { newSong } = this.state;
        newSong[name] = event.target.value;
        this.setState({ newSong });
    };

    handleEditSongChange = event => {
        event.preventDefault();
        const name = event.target.name;
        const { editSong } = this.state;
        editSong[name] = event.target.value;
        this.setState({ editSong });
    };

    toggleNewSongModal = () => {
        this.setState({
            newSongModal: !this.state.newSongModal
        });
    };
    toggleEditSongModal = () => {
        this.setState({
            editSongModal: !this.state.editSongModal
        });
    };

    addSong = () => {
        let { newSong, user } = this.state;
        newSong.User.UserId = user.id;
        songService.postSong(newSong).then(song => {
            let { data } = this.state;
            data.songs.push(song);
            this.setState({
                data,
                newSongModal: false,
                editSongModal: false,
                newSong: {
                    Text: "",
                    Playlist: {
                        Id: this.state.playlistId
                    },
                    User: {
                        UserId: ""
                    }
                },
                editSong: {}
            });
        });
    };

    updateSong = () => {
        const { editSong } = this.state;
        songService.putSong(editSong).then(response => {
            this._refreshPlaylist();
            this.setState({
                editSongModal: !this.state.editSongModal
            });
        });
    };

    editSong(song) {
        this.setState({
            editSongModal: !this.state.editSongModal,
            editSong: { ...song }
        });
    }

    deleteSong(id) {
        songService.deleteSong(id).then(response => {
            this._refreshPlaylist();
        });
    }

    componentDidMount() {
        this._refreshPlaylist();
    }
    _refreshPlaylist = () => {
        playlistService
            .getPlaylist(this.state.playlistId)
            .then(data => this.setState({ data: data, isLoaded: true }));
    };

    render() {
        const { data, isLoaded, user } = this.state;
        return (
            <div className="App container">
                <h1>Playlist "{data.Text}" page</h1>
                <div>
                    <Label htmlFor="text">Playlist text</Label>
                    <Input
                        name="text"
                        type="textarea"
                        className="form-control"
                        value={data.Description}
                        readOnly={true}
                    />
                </div>
                <div>
                    <Button
                        className="my-3"
                        color="primary"
                        onClick={this.toggleNewSongModal}
                    >
                        Add song
                    </Button>

                    <Modal
                        isOpen={this.state.newSongModal}
                        toggle={this.toggleNewSongModal}
                    >
                        <ModalHeader toggle={this.toggleNewSongModal}>
                            Add a new song
                        </ModalHeader>
                        <ModalBody>
                            <FormGroup>
                                <Label for="rating">Text</Label>
                                <Input
                                    id="text"
                                    name="Text"
                                    type="textarea"
                                    value={this.state.newSong.Text}
                                    onChange={this.handleNewSongChange}
                                />
                            </FormGroup>
                        </ModalBody>
                        <ModalFooter>
                            <Button color="primary" onClick={this.addSong}>
                                Add song
                            </Button>{" "}
                            <Button
                                color="secondary"
                                onClick={this.toggleNewSongModal}
                            >
                                Cancel
                            </Button>
                        </ModalFooter>
                    </Modal>

                    <Modal
                        isOpen={this.state.editSongModal}
                        toggle={this.toggleEditSongModal}
                    >
                        <ModalHeader toggle={this.toggleEditSongModal}>
                            Edit song
                        </ModalHeader>
                        <ModalBody>
                            <FormGroup>
                                <Label for="text">Text</Label>
                                <Input
                                    id="text"
                                    name="text"
                                    value={this.state.editSong.text}
                                    onChange={this.handleEditSongChange}
                                />
                            </FormGroup>
                        </ModalBody>
                        <ModalFooter>
                            <Button
                                color="primary"
                                onClick={this.updateSong}
                            >
                                Update song
                            </Button>{" "}
                            <Button
                                color="secondary"
                                onClick={this.toggleEditSongModal}
                            >
                                Cancel
                            </Button>
                        </ModalFooter>
                    </Modal>

                    <Table>
                        <thead>
                            <tr>
                                <th>Text</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {isLoaded ? (
                                data.songs.map(song => (
                                    <tr key={song.id}>
                                        <td>{song.text}</td>
                                        <td>
                                            {user.id == song.userId ? (
                                                <Button
                                                    color="success"
                                                    size="sm"
                                                    className="mr-2"
                                                    onClick={this.editSong.bind(
                                                        this,
                                                        song
                                                    )}
                                                >
                                                    Edit
                                                </Button>
                                            ) : (
                                                ""
                                            )}
                                            {user.role === "Admin" ||
                                            user.id == data.userId ? (
                                                <Button
                                                    color="danger"
                                                    size="sm"
                                                    onClick={this.deleteSong.bind(
                                                        this,
                                                        song.id
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

export default Playlist;
