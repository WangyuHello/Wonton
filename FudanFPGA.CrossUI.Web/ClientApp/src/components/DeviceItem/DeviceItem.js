import React, { Component } from 'react';
import { Card, CardBody, CardTitle, CardSubtitle, CardLink, Button } from 'reactstrap';

import './DeviceItem.css';

export class DeviceItem extends Component {
    
    render() {
        return (
            <Card style={{marginTop: '20px'}}>
                <CardBody>
                    <CardTitle>LED</CardTitle>
                    <CardSubtitle>输出</CardSubtitle>
                </CardBody>
                <div style={{height: '20px', backgroundColor: 'gray'}}></div>
                <CardBody>
                    <Button >添加</Button>
                </CardBody>
            </Card>
        );
    }
}