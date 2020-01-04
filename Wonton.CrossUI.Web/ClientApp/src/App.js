import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

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
