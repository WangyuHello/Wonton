import React, { Component } from 'react';
import { Title } from './Title';
import { MainPanel } from './MainPanel/MainPanel';
import { pjManager } from './Service/ProjectManager';
import { darkmode } from "./Service/Darkmode";

import './Layout.css'

export class Layout extends Component {

  state = {
    titleData: null,
    panelData: null,
    modified: false,
    isGalleryOpen: false
  }

  async componentDidMount() {
    let data = await pjManager.GetInitData();
    console.log(data)
    this.setState({
      titleData: {isDarkMode: data.isDarkMode, ...data.titleData},
      panelData: data.panelData
    });
    if (data.isDarkMode) {
      darkmode.ActivateDarkMode();
    }
    this.forceUpdate();
  }

  onModified = (event) => {
    this.setState({
      modified: event
    });
  }

  onGalleryToggle = () => {
    this.setState(prev => ({isGalleryOpen: !prev.isGalleryOpen}))
  }

  onGalleryStateChange = (state) => {
    this.setState({isGalleryOpen: state.isOpen})
  }

  render () {
    return (
      <div className="AppRoot">
        <Title titleData={this.state.titleData} modified={this.state.modified} onModified={this.onModified} onGalleryToggle={this.onGalleryToggle}/>
        <MainPanel panelData={this.state.panelData} onModified={this.onModified} isGalleryOpen={this.state.isGalleryOpen} onGalleryStateChange={this.onGalleryStateChange}/>
      </div>
    );
  }
}
