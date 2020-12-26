import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {

    title = '';
    userData$: Observable<any>;
    isAuthenticated = false;

    constructor(private oidcSecurityService: OidcSecurityService) {
        console.log('AppComponent STARTING');
    }

    ngOnInit(): void {
        this.userData$ = this.oidcSecurityService.userData$;

        this.oidcSecurityService.checkAuth().subscribe((isAuthenticated) => {
            this.isAuthenticated = isAuthenticated;
            console.log('app authenticated', isAuthenticated);
        });
    }
    login(): void {
        console.log('start login');

        this.oidcSecurityService.authorize();
    }

    refreshSession(): void {
        console.log('start refreshSession');
        this.oidcSecurityService.authorize();
    }

    logoffAndRevokeTokens(): void {
        this.oidcSecurityService.logoffAndRevokeTokens().subscribe((result) => console.log(result));
    }

    revokeRefreshToken(): void {
        this.oidcSecurityService.revokeRefreshToken().subscribe((result) => console.log(result));
    }

    revokeAccessToken(): void {
        this.oidcSecurityService.revokeAccessToken().subscribe((result) => console.log(result));
    }
}
