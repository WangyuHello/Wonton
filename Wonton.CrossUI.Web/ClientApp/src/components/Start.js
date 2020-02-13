import React, { Component } from 'react';
import { Button, ListGroup, ListGroupItem } from 'reactstrap';

import './Start.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPlus, faFolderOpen } from '@fortawesome/free-solid-svg-icons';

export class Start extends Component {

    onOpen = (event) => {
        this.props.onOpen(event);
    }

    onNew = (event) => {
        this.props.onNew(event);
    }

  render () {
    return (
      <div className="twocolumn">
          <div className="buttons">
            <Button size="lg" onClick={this.onNew}>
                <FontAwesomeIcon icon={faPlus} />
            </Button>
            <Button size="lg" onClick={this.onOpen}>
                <FontAwesomeIcon icon={faFolderOpen} />
            </Button>
          </div>
          <div className="recent"> 
                <div style={{marginBottom: "10px"}}>最近项目</div>
                <div>
                <ListGroup>
                    <ListGroupItem>Cras justo odio</ListGroupItem>
                    <ListGroupItem>Dapibus ac facilisis in</ListGroupItem>
                    <ListGroupItem>Morbi leo risus</ListGroupItem>
                    <ListGroupItem>Porta ac consectetur ac</ListGroupItem>
                    <ListGroupItem>Vestibulum at eros</ListGroupItem>
                </ListGroup>
                </div>
          </div>
      </div>
    );
  }
}
