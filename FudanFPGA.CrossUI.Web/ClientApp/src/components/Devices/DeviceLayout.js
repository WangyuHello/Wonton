import React, { Component } from 'react';
import { Button } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog, faTimes } from '@fortawesome/free-solid-svg-icons'

export class DeviceLayout extends Component {

    render() {
        return (
            <div style={{height: "100%", display:"flex", flexDirection: "column", justifyContent:"space-between"}}>
                <div style={{display:"flex", justifyContent:"space-between", marginLeft: "10px", marginRight: "4px"}}>
                    <div>{this.props.instance}</div>
                    <div style={{display: "flex"}}>
                        <div>
                            <Button outline color='secondary' size='sm' style={{borderRadius: "0", paddingTop: "0", paddingBottom: "0", paddingLeft: '4px', paddingRight:"4px", borderWidth:"0px" }}>
                                <FontAwesomeIcon icon={faCog}/>
                            </Button>
                        </div>
                        <div>
                            <Button outline color='danger' size='sm' style={{borderRadius: "0", paddingTop: "0", paddingBottom: "0", paddingLeft: '4px', paddingRight:"4px", marginLeft:"4px", borderWidth:"0px" }}>
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
                <div></div>
            </div>
        );
    }
}