import React, { Component } from 'react';
import { Button } from 'reactstrap'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMusic } from '@fortawesome/free-solid-svg-icons'

import './HButton.css';

export class HButtonCore extends Component {
    
    static defaultProps = {
        name: "按钮",
        ClassName: "HButton"
    }


    render() {
        return (      
            <Button outline active={false} size='lg' className="myToggleButton">
                <FontAwesomeIcon icon={faMusic}/>
            </Button> 
        );
    }
}