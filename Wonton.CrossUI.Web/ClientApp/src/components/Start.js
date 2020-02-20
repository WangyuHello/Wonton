import React, { Component } from 'react';
import { Button, ListGroup, ListGroupItem, UncontrolledTooltip  } from 'reactstrap';

import './Start.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPlus, faFolderOpen } from '@fortawesome/free-solid-svg-icons';

import { pjManager } from './Service/ProjectManager';

export class Start extends Component {

  onOpen = (event) => {
      this.props.onOpen(event);
  }

  onNew = (event) => {
      this.props.onNew(event);
  }

  clickOpen = async (index) => {
    console.log('Open project index:' + index)

    //将新的项目地址存储在后台进程
    pjManager.projectFile = this.props.recentProjects[index].Item2;
    await pjManager.SetProjectFile();

    //刷新页面,重新载入项目
    window.location.reload(true);
  }

  render () {

    console.log(this.props.recentProjects)

    return (
      <div className="twocolumn">
          <div className="buttons">
            <Button className="tranButton" size="lg" onClick={this.onNew}>
                <FontAwesomeIcon icon={faPlus} />
            </Button>
            <Button className="tranButton" size="lg" onClick={this.onOpen}>
                <FontAwesomeIcon icon={faFolderOpen} />
            </Button>
          </div>
          <div className="recent"> 
                <div style={{marginBottom: "10px"}}>最近项目</div>
                <ListGroup className="startList">
                    {
                      this.props.recentProjects.map((item, ind) => {
                      return <ListGroupItem key={ind} onClick={event => this.clickOpen(ind)} tag="a" href="#" action id={"Tooltip-" + ind}>
                              {item.Item1} 
                              <UncontrolledTooltip  placement="top" target={"Tooltip-" + ind} fade="true">{item.Item2}</UncontrolledTooltip >
                        </ListGroupItem>
                      })
                    }
                </ListGroup>
          </div>
      </div>
    );
  }
}
