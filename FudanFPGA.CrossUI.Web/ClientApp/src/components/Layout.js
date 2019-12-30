import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { Title } from './Title';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <Title />
        <NavMenu />
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
