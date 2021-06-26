import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';


import { AppComponent } from './app.component';
import { MastercardsFgComponent } from './mastercards-fg/mastercards-fg.component';
import { AppRoutingModule } from './app-routing/app-routing.module';



import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MastercardPdsComponent } from './mastercard-pds/mastercard-pds.component';

@NgModule({
  declarations: [
    AppComponent,
    MastercardsFgComponent,
    MastercardPdsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
