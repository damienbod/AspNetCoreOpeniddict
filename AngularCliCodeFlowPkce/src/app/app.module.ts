import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { routing } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { DataEventRecordsModule } from './dataeventrecords/dataeventrecords.module';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { AuthModule, LogLevel, OidcSecurityService } from 'angular-auth-oidc-client';

export function configureAuth(oidcSecurityService: OidcSecurityService) {
  return () => oidcSecurityService.checkAuth();
}

@NgModule({ declarations: [
        AppComponent,
        ForbiddenComponent,
        HomeComponent,
        UnauthorizedComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        FormsModule,
        routing,
        DataEventRecordsModule,
        AuthModule.forRoot({
            config: {
                authority: 'https://localhost:44395',
                redirectUrl: window.location.origin,
                postLogoutRedirectUri: window.location.origin,
                clientId: 'angularclient',
                scope: 'openid profile email dataEventRecords offline_access',
                responseType: 'code',
                silentRenew: true,
                renewTimeBeforeTokenExpiresInSeconds: 10,
                useRefreshToken: true,
                logLevel: LogLevel.Debug,
            },
        })], providers: [
        provideHttpClient(withInterceptorsFromDi()),
        {
            provide: APP_INITIALIZER,
            useFactory: configureAuth,
            deps: [OidcSecurityService],
            multi: true,
        }
    ] })

export class AppModule {
  constructor() {
      console.log('APP STARTING');
  }
}
