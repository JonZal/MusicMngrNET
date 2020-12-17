import React, { Component } from "react";
import { authenticationService } from "../services/authentication";
import { songService } from "../services/songService";
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

class MySongs extends Component {
    constructor(props) {
        super(props);
        if (!authenticationService.currentUserValue) {
            this.props.history.push("/");
        }
        this.state = {
            data: {},
            isLoaded: false,
            editSong: {},
            editSongModal: false,
            user: jwt(authenticationService.currentUserValue.token)
        };
    }

    handleEditSongChange = event => {
        event.preventDefault();
        const name = event.target.name;
        const { editSong } = this.state;
        editSong[name] = event.target.value;
        this.setState({ editSong });
    };

    toggleEditSongModal = () => {
        this.setState({
            editSongModal: !this.state.editSongModal
        });
    };

    updateSong = () => {
        const { editSong } = this.state;
        songService.putSong(editSong).then(response => {
            this._refreshSongs();
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
            this._refreshSongs();
        });
    }

    componentDidMount() {
        this._refreshSongs();
    }

    _refreshSongs = () => {
        userService
            .getCurrentUserSongs()
            .then(data => this.setState({ data: data, isLoaded: true }));
    };

    render() {
        const { isLoaded, data } = this.state;
        return (
            <div className="App container">
                <h1>My songs page</h1>

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
                        <Button color="primary" onClick={this.updateSong}>
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

                <Table className="table">
                    <thead>
                        <tr>
                            <th>Id</th>
                            <th>Text</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoaded ? (
                            data.map(song => (
                                <tr key={song.id}>
                                    <td>{song.id}</td>
                                    <td>{song.text}</td>
                                    <td>
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

export default MySongs;
