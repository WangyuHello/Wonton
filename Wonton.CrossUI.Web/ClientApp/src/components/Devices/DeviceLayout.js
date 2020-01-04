import React, { Component } from 'react';
import { Button } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog, faTimes } from '@fortawesome/free-solid-svg-icons';

import './DeviceLayout.css';

export class DeviceLayout extends Component {

    OnSettingClick = (event, instance) => {
        this.props.OnSettingClick(event, instance);
    }

    OnRemoveClick = (event, instance) => {
        this.props.OnRemoveClick(event, instance);
    }

    render() {



        return (
            <div style={{height: "100%", display:"flex", flexDirection: "column", justifyContent:"space-between"}}>
                <div style={{display:"flex", justifyContent:"space-between", marginLeft: "10px", marginRight: "4px"}}>
                    <div>{this.props.instance}</div>
                    <div style={{display: "flex"}}>
                        <div>
                            <Button outline color='secondary' size='sm' style={{borderRadius: "0", paddingTop: "0", paddingBottom: "0", paddingLeft: '4px', paddingRight:"4px", borderWidth:"0px",marginTop:"-4px" }} className="FramelessBtn" onClick={this.OnSettingClick.bind(this, this.props.children)}>
                                <FontAwesomeIcon icon={faCog}/>
                            </Button>
                        </div>
                        <div>
                            <Button outline color='danger' size='sm' style={{borderRadius: "0", paddingTop: "0", paddingBottom: "0", paddingLeft: '4px', paddingRight:"4px", marginLeft:"4px", borderWidth:"0px",marginTop:"-4px" }} className="FramelessBtn" onClick={this.OnRemoveClick.bind(this, this.props.children)}>
                                <FontAwesomeIcon icon={faTimes}/>
                            </Button>
                        </div>
                    </div>
                </div>
                <div style={{display:"flex", justifyContent: "space-between", marginTop:"-16px"}}>
                    <div></div>
                    {this.props.children}
                    <div></div>
                </div>
                <div style={{display:"flex", justifyContent:"flex-start", marginLeft: "10px"}}></div>
            </div>
        );
    }
}