import React, { Component } from 'react';
import { Layout } from './components/Layout';

import './custom.css'
import 'react-grid-layout/css/styles.css'
import 'react-resizable/css/styles.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>

      </Layout>
    );
  }
}
