import React, { Component } from 'react';
import { Card, CardBody, CardTitle, Button } from 'reactstrap';

import './DeviceItem.css';

export class DeviceItem extends Component {

    OnAdd = (event, name) => {
        this.props.OnAdd(event, name);
    }
    
    render() {
        return (
            <Card style={{marginTop: '20px'}} >
                <CardBody>
                    <CardTitle>{this.props.children.props.name}</CardTitle>
                </CardBody>
                    <div style={{textAlign: "center"}}>
                        {this.props.children}
                    </div>
                <CardBody>
                    <Button onClick={(e) => this.OnAdd(e, this.props.children.props.ClassName)}>添加</Button>
                </CardBody>
            </Card>
        );
    }
}