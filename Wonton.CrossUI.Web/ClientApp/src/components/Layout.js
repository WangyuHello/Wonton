import React, { Component } from 'react';
import { Title } from './Title';
import { MainPanel } from './MainPanel/MainPanel';
import { pjManager } from './Service/ProjectManager';

import './Layout.css'

export class Layout extends Component {

  state = {
    titleData: null,
    panelData: null,
    modified: false
  }

  async componentDidMount() {
    let data = await pjManager.GetInitData();
    console.log(data)
    this.setState({
      titleData: data.titleData,
      panelData: data.panelData
    });

    this.forceUpdate();
  }

  onModified = (event) => {
    this.setState({
      modified: event
    });
  }

  render () {
    return (
      <div className="AppRoot">
        <Title titleData={this.state.titleData} modified={this.state.modified} onModified={this.onModified}/>
        <MainPanel panelData={this.state.panelData} onModified={this.onModified}/>
      </div>
    );
  }
}
