import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import './app.component.css';

@Component({
    selector: 'my-app',
    templateUrl: 'app.component.html'
})

export class AppComponent implements OnInit {

    constructor(public securityService: OidcSecurityService) {
    }

    ngOnInit() {
        if (window.location.hash) {
            this.securityService.authorizedCallback();
        }
    }

    public Login() {
        console.log('Do login logic');
        this.securityService.authorize();
    }

    public Logout() {
        console.log('Do logout logic');
        this.securityService.logoff();
    }
}