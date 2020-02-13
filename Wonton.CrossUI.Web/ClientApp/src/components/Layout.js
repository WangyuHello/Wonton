import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { Title } from './Title';
import { MainPanel } from './MainPanel/MainPanel';

import './Layout.css'

export class Layout extends Component {

  render () {
    return (
      <div className="AppRoot">
        <Title />
        <MainPanel />
      </div>
    );
  }
}
